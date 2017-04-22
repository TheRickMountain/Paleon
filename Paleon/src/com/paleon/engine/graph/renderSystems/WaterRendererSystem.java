package com.paleon.engine.graph.renderSystems;

import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.water.WaterFrameBuffers;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.textures.Texture;

public class WaterRendererSystem {

	private static final float WAVE_SPEED = 0.03f;
	
	private ShaderProgram shader;
	private Mesh mesh;
	
	private Texture dudvMap;
	private Texture normalMap;
	
	private float moveFactor = 0;
	
	public WaterRendererSystem(Camera camera) {
		shader = ResourceManager.loadShader("water");
		
		shader.createUniform("modelMatrix");
		shader.createUniform("viewMatrix");
		shader.createUniform("projectionMatrix");
		shader.createUniform("reflectionTexture");
		shader.createUniform("refractionTexture");
		shader.createUniform("dudvMap");
		shader.createUniform("normalMap");
		shader.createUniform("depthMap");
		shader.createUniform("moveFactor");
		shader.createUniform("cameraPosition");
		shader.createUniform("lightPosition");
		shader.createUniform("lightColor");
		shader.createUniform("fogColor");
		
		shader.setUniform("reflectionTexture", 0, true);
		shader.setUniform("refractionTexture", 1, true);
		shader.setUniform("dudvMap", 2, true);
		shader.setUniform("normalMap", 3, true);
		shader.setUniform("depthMap", 4, true);
		shader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
		
		float[] vertices = { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 };
		mesh = new Mesh(vertices, 2);
		
		dudvMap = ResourceManager.getTexture("dudvMap");
		normalMap = ResourceManager.getTexture("normalMap");
	}
	
	public void update(float deltaTime) {
		moveFactor += WAVE_SPEED * deltaTime;
		moveFactor %= 1;
	}
	
	public void render(List<WaterTile> waters, Camera camera, Light light, Color fogColor, WaterFrameBuffers fbos) {
		if(Display.wasResized()){
			shader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
		}
		
		prepareRender(camera, light, fogColor, fbos);
		
		for(WaterTile waterTile : waters) {
			if(camera.getFrusutmCuller().testWaterTileInView(waterTile)) {
				Matrix4f modelMatrix = MathUtils.getModelMatrix(
						new Vector3f(waterTile.getX(), waterTile.getHeight(), waterTile.getZ()), 0, 0, 0, WaterTile.TILE_SIZE);
				shader.setUniform("modelMatrix", modelMatrix);
				GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, mesh.getVertexCount());
			}
		}
		GL11.glDisable(GL11.GL_BLEND);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		shader.unbind();
	}
	
	public void prepareRender(Camera camera, Light light, Color fogColor, WaterFrameBuffers fbos){
		shader.bind();
		shader.setUniform("viewMatrix", camera.getViewMatrix());
		shader.setUniform("moveFactor", moveFactor);
		shader.setUniform("cameraPosition", camera.getPosition());
		shader.setUniform("lightPosition", light.getPosition());
		shader.setUniform("lightColor", light.getDiffuse());
		shader.setUniform("fogColor", fogColor);
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
		
		GL11.glEnable(GL11.GL_BLEND);
		GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
	}
	
	public void cleanup() {
		shader.cleanup();
		mesh.cleanup();
	}
	
}
