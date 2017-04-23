package com.paleon.engine.graph.renderer;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL32;

import com.paleon.engine.Display;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Light;
import com.paleon.engine.processing.LodCalculator;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.terrain.TexturePack;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class TerrainRenderer {

	private Camera camera;
	
	private TerrainShader shader;
	
	public TerrainRenderer(Camera camera) {
		this.camera = camera;
		shader = new TerrainShader();
		
		shader.start();
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.stop();
	}

	public void render(Map<Terrain, List<TerrainBlock>> terrainBatches, Light light, Color fogColor, Vector4f plane) {
		shader.start();
		
		if(Display.wasResized()) {
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
		
		shader.plane.loadVec4(plane);
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		shader.lightPosition.loadVec3(light.getPosition());
		shader.lightColor.loadColor(light.getDiffuse());
		shader.fogColor.loadColor(fogColor);
		
		render(terrainBatches);
		
		shader.stop();
	}
	
	private void render(Map<Terrain, List<TerrainBlock>> terrainBatches) {
		for (List<TerrainBlock> blocks : terrainBatches.values()) {
			for (TerrainBlock block : blocks) {
				block.setStitching();
				LodCalculator.calculateTerrainLOD(block, camera);
			}
		}
		
		OpenglUtils.cullFace(true);
		for(Terrain terrain : terrainBatches.keySet()) {
			prepareTerrainInstance(terrain);
			List<TerrainBlock> batch = terrainBatches.get(terrain);
			for(TerrainBlock terrainBlock : batch) {
				if(camera.getFrusutmCuller().testTerrainInView(terrainBlock)) {
					int[] indexInfo = terrainBlock.getIndicesVBOInfo();
					OpenglUtils.bindIndicesVBO(indexInfo[0]);
					render(terrainBlock.getIndex(), indexInfo[1]);
					OpenglUtils.unbindIndicesVBO();
				}
			}
			OpenglUtils.unbindVAO();
			GL11.glBindTexture(GL11.GL_TEXTURE_2D, 0);
		}
	}
	
	private void prepareTerrainInstance(Terrain terrain) {	
		shader.modelMatrix.loadMatrix(MathUtils.getModelMatrix(new Vector3f(terrain.getX(), 0, terrain.getZ()), 
				0, 0, 0, 1));
		
		TexturePack texturePack = terrain.getTexture();
		texturePack.getBlendMap().bindToUnit(0);
		texturePack.getaTexture().bindToUnit(1);
		texturePack.getrTexture().bindToUnit(2);
		texturePack.getgTexture().bindToUnit(3);
		texturePack.getbTexture().bindToUnit(4);
		
		OpenglUtils.bindVAO(terrain.getVaoId());
	}
	
	private void render(int blockIndex, int indicesLength) {
		int vertexOffset = blockIndex * Terrain.VERTICES_PER_NODE;
		GL32.glDrawElementsBaseVertex(GL11.GL_TRIANGLES, indicesLength, GL11.GL_UNSIGNED_INT, 0, vertexOffset);
	}
	
	public void cleanup() {
		shader.cleanup();
	}
	
}
