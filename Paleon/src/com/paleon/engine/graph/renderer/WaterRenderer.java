package com.paleon.engine.graph.renderer;

import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.water.WaterFrameBuffers;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.textures.Texture;

public class WaterRenderer {

	private static final float WAVE_SPEED = 0.03f;
	
	private Camera camera;
	
	private WaterShader shader;
	private Mesh mesh;
	
	private Texture dudvMap;
	private Texture normalMap;
	
	private float moveFactor = 0;
	
	public WaterRenderer(Camera camera) {
		this.camera = camera;
		shader = new WaterShader();
		
		shader.start();
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.stop();
		
		float[] vertices = { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 };
		mesh = new Mesh(vertices, 2);
		
		dudvMap = ResourceManager.getTexture("dudvMap");
		normalMap = ResourceManager.getTexture("normalMap");
	}
	
	public void update(float deltaTime) {
		moveFactor += WAVE_SPEED * deltaTime;
		moveFactor %= 1;
	}
	
	public void render(List<WaterTile> waters, Light light, Color fogColor, WaterFrameBuffers fbos) {
		shader.start();
		
		if(Display.wasResized()){
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
		
		prepareRender(light, fogColor, fbos);
		
		for(WaterTile waterTile : waters) {
			if(camera.getFrusutmCuller().testWaterTileInView(waterTile)) {
				Matrix4f modelMatrix = MathUtils.getModelMatrix(
						new Vector3f(waterTile.getX(), waterTile.getHeight(), waterTile.getZ()), 0, 0, 0, WaterTile.TILE_SIZE);
				shader.modelMatrix.loadMatrix(modelMatrix);
				GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, mesh.getVertexCount());
			}
		}
		OpenglUtils.alphaBlending(false);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		shader.stop();
	}
	
	public void prepareRender(Light light, Color fogColor, WaterFrameBuffers fbos){
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		shader.moveFactor.loadFloat(moveFactor);
		shader.cameraPosition.loadVec3(camera.getPosition());
		shader.lightPosition.loadVec3(light.getPosition());
		shader.lightColor.loadColor(light.getDiffuse());
		shader.fogColor.loadColor(fogColor);
		GL13.glActiveTexture(GL13.GL_TEXTURE0);
		GL11.glBindTexture(GL11.GL_TEXTURE_2D, fbos.getReflectionTexture());
		GL13.glActiveTexture(GL13.GL_TEXTURE1);
		GL11.glBindTexture(GL11.GL_TEXTURE_2D, fbos.getRefractionTexture());
		dudvMap.bindToUnit(2);
		normalMap.bindToUnit(3);
		GL13.glActiveTexture(GL13.GL_TEXTURE4);
		GL11.glBindTexture(GL11.GL_TEXTURE_2D, fbos.getRefractionDepthTexture());
		GL30.glBindVertexArray(mesh.getVAO());
		GL20.glEnableVertexAttribArray(0);
		
		OpenglUtils.alphaBlending(true);
	}
	
	public void cleanup() {
		shader.cleanup();
		mesh.cleanup();
	}
	
}
