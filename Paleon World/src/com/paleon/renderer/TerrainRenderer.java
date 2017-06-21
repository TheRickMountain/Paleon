package com.paleon.renderer;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL30;
import org.lwjgl.opengl.GL32;

import com.paleon.core.Camera;
import com.paleon.core.Display;
import com.paleon.math.Matrix4f;
import com.paleon.terrain.Terrain;
import com.paleon.terrain.TerrainBlock;
import com.paleon.toolbox.LodCalculator;
import com.paleon.toolbox.MathUtils;


public class TerrainRenderer {

	private Camera camera;
	
	private TerrainShader shader;
	
	private Matrix4f modelMatrix = new Matrix4f();
	
	public TerrainRenderer(Camera camera) {
		this.camera = camera;
		shader = new TerrainShader();
	}
	
	public void render(Map<Terrain, List<TerrainBlock>> terrainBatches) {		
		shader.start();
		
		if(Display.isResized()) {
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
		
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		
		for (List<TerrainBlock> blocks : terrainBatches.values()) {
			for (TerrainBlock block : blocks) {
				block.setStitching();
				LodCalculator.calculateTerrainLOD(block, camera);
			}
		}
		
		for(Terrain terrain : terrainBatches.keySet()) {
			prepareTerrainInstance(terrain);
			List<TerrainBlock> batch = terrainBatches.get(terrain);
			for(TerrainBlock terrainBlock : batch) {
				//if(camera.getFrusutmCuller().testTerrainInView(terrainBlock)) {
					int[] indexInfo = terrainBlock.getIndicesVBOInfo();
					GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, indexInfo[0]);
					render(terrainBlock.getIndex(), indexInfo[1]);
					GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, 0);
				//}
			}
			GL30.glBindVertexArray(0);
		}
		shader.stop();
	}
	
	
	private void prepareTerrainInstance(Terrain terrain) {	
		shader.modelMatrix.loadMatrix(MathUtils.getModelMatrix(modelMatrix, 
				terrain.getX(), 0, terrain.getZ(), 0, 0, 0, 1, 1, 1));
		
		GL30.glBindVertexArray(terrain.getVaoId());
	}
	
	private void render(int blockIndex, int indicesLength) {
		int vertexOffset = blockIndex * Terrain.VERTICES_PER_NODE;
		GL32.glDrawElementsBaseVertex(GL11.GL_TRIANGLES, indicesLength, GL11.GL_UNSIGNED_INT, 0, vertexOffset);
	}
	
	public void cleanup() {
		shader.cleanup();
	}
}
