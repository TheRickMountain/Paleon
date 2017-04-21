package com.paleon.engine.graph.renderSystems;

import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.Image;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.toolbox.Rect;
import com.paleon.maths.vecmath.Matrix4f;

public class ImageRendererSystem {

	private ShaderProgram shader;
	
	Mesh mesh;
	
	private Matrix4f projectionMatrix = new Matrix4f();
	
	public ImageRendererSystem() {
		initRenderData();		
		
		shader = ResourceManager.loadShader("sprite");
		
		shader.createUniform("spriteColor");
		shader.createUniform("image");
		shader.createUniform("MP");
		shader.createUniform("solidColor");
		
		MathUtils.getOrtho2DProjectionMatrix(projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0);
		shader.setUniform("image", 0, true);
	}
	
	public void render(List<GameObject> gameObjects) {
		if(Display.wasResized()) {
			MathUtils.getOrtho2DProjectionMatrix(projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0);
		}
		
		shader.bind();
		
		OpenglUtils.cullFace(true);
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		for(GameObject gameObject : gameObjects) {
			Image image = gameObject.getComponent(Image.class);
			int textureId = image.textureId;
			Color color = image.color;
			
			shader.setUniform("spriteColor", color);
			shader.setUniform("MP", 
					Matrix4f.mul(projectionMatrix, 
							MathUtils.getModelMatrix(gameObject.position.x, gameObject.position.y, 
									gameObject.rotation.z, 
									gameObject.scale.x, gameObject.scale.y), null));
			
			if(textureId == 0) {
				shader.setUniform("solidColor", 1);
			} else {
				GL13.glActiveTexture(GL13.GL_TEXTURE0);
				GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
				shader.setUniform("solidColor", 0);
			}
			
			GL30.glBindVertexArray(mesh.getVaoId());
			GL20.glEnableVertexAttribArray(0);
			GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, 4);
			GL20.glDisableVertexAttribArray(0);
			GL30.glBindVertexArray(0);
		}
		
		OpenglUtils.cullFace(false);
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
		
		shader.unbind();
	}
	
	public void render(Rect rect, int textureId) {
		shader.bind();
		
		OpenglUtils.cullFace(true);
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		shader.setUniform("spriteColor", Color.WHITE);
		shader.setUniform("MP", 
				Matrix4f.mul(projectionMatrix, 
						MathUtils.getModelMatrix(rect.x, rect.y, 
								0, rect.width, rect.height), null));
		
		if(textureId == 0) {
			shader.setUniform("solidColor", 1);
		} else {
			GL13.glActiveTexture(GL13.GL_TEXTURE0);
			GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
			shader.setUniform("solidColor", 0);
		}
		
		GL30.glBindVertexArray(mesh.getVaoId());
		GL20.glEnableVertexAttribArray(0);
		GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, 4);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		
		OpenglUtils.cullFace(false);
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
		
		shader.unbind();
	}
	
	public void render(GameObject gameObject) {
		if(Display.wasResized()) {
			MathUtils.getOrtho2DProjectionMatrix(projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0);
			shader.setUniform("projection", projectionMatrix, true);
		}
		
		shader.bind();
		
		Image image = gameObject.getComponent(Image.class);
		int textureId = image.textureId;
		Color color = image.color;
		
		OpenglUtils.cullFace(true);
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		shader.setUniform("spriteColor", color);
		shader.setUniform("MP", 
				Matrix4f.mul(projectionMatrix, 
						MathUtils.getModelMatrix(gameObject.position.x, gameObject.position.y, 
								gameObject.rotation.z, 
								gameObject.scale.x, gameObject.scale.y), null));
		
		if(textureId == 0) {
			shader.setUniform("solidColor", 1);
		} else {
			GL13.glActiveTexture(GL13.GL_TEXTURE0);
			GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
			shader.setUniform("solidColor", 0);
		}
		
		GL30.glBindVertexArray(mesh.getVaoId());
		GL20.glEnableVertexAttribArray(0);
		GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, 4);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		
		OpenglUtils.cullFace(false);
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
		
		shader.unbind();
	}

	private void initRenderData() {
		float[] data = new float[] {
				0f, 0f, 0.0f, 0.0f,
				0f, 1f, 0.0f, 1.0f,
				1f, 0f, 1.0f, 0.0f,
				1f, 1f, 1.0f, 1.0f
		};
		
		mesh = new Mesh(data, 4);
	}
	
	public void cleanup() {
		mesh.cleanup();
		
		shader.cleanup();
	}
	
}
