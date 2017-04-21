package com.paleon.engine.graph.renderSystems;

import java.io.File;
import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.graph.font.FontType;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.toolbox.Rect;
import com.paleon.maths.vecmath.Matrix4f;

public class GUIRendererSystem {

	public ShaderProgram shader;
	
	public Mesh mesh;
	
	public Matrix4f projectionMatrix = new Matrix4f();
	
	public static FontType primitiveFont;
	
	public GUIRendererSystem() {
		initRenderData();
		
		shader = ResourceManager.loadShader("gui");
		
		shader.createUniform("spriteColor");
		shader.createUniform("image");
		shader.createUniform("mode");
		shader.createUniform("MP");

		shader.setUniform("image", 0);

		MathUtils.getOrtho2DProjectionMatrix(projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0);
		
		primitiveFont = new FontType(ResourceManager.getTexture("primitive_font"), new File("res/primitive_font.fnt"));
	}
	
	public void render(List<GameObject> gameObjects) {
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		if(Display.wasResized()) {
			MathUtils.getOrtho2DProjectionMatrix(projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0);
		}

		shader.bind();
		
		for(GameObject gameObject : gameObjects) {
			if(gameObject.isActive()) {
				Image image = gameObject.getComponent(Image.class);
				if(image != null) {
					if(image.enabled) {
						int textureId = image.textureId;
						Color color = image.color;
						
						shader.setUniform("spriteColor", color);
						shader.setUniform("MP", 
								Matrix4f.mul(projectionMatrix, 
										MathUtils.getModelMatrix(gameObject.position.x, gameObject.position.y, 
												gameObject.rotation.z, 
												gameObject.scale.x, gameObject.scale.y), null));
						
						if(textureId == 0) {
							shader.setUniform("mode", 1);
						} else {
							GL13.glActiveTexture(GL13.GL_TEXTURE0);
							GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
							shader.setUniform("mode", 0);
						}
						
						GL30.glBindVertexArray(mesh.getVaoId());
						GL20.glEnableVertexAttribArray(0);
						GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, 4);
						GL20.glDisableVertexAttribArray(0);
						GL30.glBindVertexArray(0);
					}
				}
				
				Text text = gameObject.getComponent(Text.class);
				if(text != null) {
					if(text.enabled) {
						GL13.glActiveTexture(GL13.GL_TEXTURE0);
			            GL11.glBindTexture(GL11.GL_TEXTURE_2D, text.font.getTextureAtlas());
			        	
			        	shader.setUniform("mode", 2);
			        	
			        	shader.setUniform("MP", 
			        			MathUtils.getModelMatrix((gameObject.position.x / Display.getWidth()) * 2.0f, 
			        					(-gameObject.position.y / Display.getHeight()) * 2.0f, 
										gameObject.rotation.z, 
										gameObject.scale.x, gameObject.scale.y));
			        	
			        	shader.setUniform("spriteColor", text.color);
			        	
			            GL30.glBindVertexArray(text.textMeshVao);
			            GL20.glEnableVertexAttribArray(0);
			            GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, text.vertexCount);
			            GL20.glDisableVertexAttribArray(0);
			            GL30.glBindVertexArray(0);
					}
				}
				
			}
		}
		
		shader.unbind();
		
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
	}
	
	public void render(Rect rect, int textureId) {
		shader.bind();
		
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		shader.setUniform("spriteColor", Color.WHITE);
		shader.setUniform("MP", 
				Matrix4f.mul(projectionMatrix, 
						MathUtils.getModelMatrix(rect.x, rect.y, 
								0, rect.width, rect.height), null));
		
		if(textureId == 0) {
			shader.setUniform("mode", 1);
		} else {
			GL13.glActiveTexture(GL13.GL_TEXTURE0);
			GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
			shader.setUniform("mode", 0);
		}
		
		GL30.glBindVertexArray(mesh.getVaoId());
		GL20.glEnableVertexAttribArray(0);
		GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, 4);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
		
		shader.unbind();
	}
	
	public void render(Rect rect, Text text) {
		shader.bind();
		
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		GL13.glActiveTexture(GL13.GL_TEXTURE0);
        GL11.glBindTexture(GL11.GL_TEXTURE_2D, text.font.getTextureAtlas());
    	
    	shader.setUniform("mode", 2);
    	
    	shader.setUniform("MP", 
    			MathUtils.getModelMatrix((rect.x / Display.getWidth()) * 2.0f, 
    					(-rect.y / Display.getHeight()) * 2.0f, 0, 1, 1));
    	
    	shader.setUniform("spriteColor", text.color);
    	
        GL30.glBindVertexArray(text.textMeshVao);
        GL20.glEnableVertexAttribArray(0);
        GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, text.vertexCount);
        GL20.glDisableVertexAttribArray(0);
        GL30.glBindVertexArray(0);
        
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
		shader.cleanup();
	}
	
}
