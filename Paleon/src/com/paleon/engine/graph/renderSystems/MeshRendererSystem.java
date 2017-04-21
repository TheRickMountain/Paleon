package com.paleon.engine.graph.renderSystems;

import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;

import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.RenderEngine;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.Light;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector4f;

public class MeshRendererSystem {

	private ShaderProgram shader;
	
	private float waving;
	private float temp = 0;
	
	public MeshRendererSystem(Camera camera) {
		shader = ResourceManager.loadShader("entity");
		
		shader.createUniform("modelMatrix");
		shader.createUniform("viewMatrix");
		shader.createUniform("projectionMatrix");
		
		shader.createUniform("objectColor");
		
		shader.createUniform("texture_sampler");
		shader.createUniform("lightPosition");
		shader.createUniform("lightColor");
		shader.createUniform("shineDamper");
		shader.createUniform("reflectivity");
		shader.createUniform("colorMode");
		shader.createUniform("useFakeLighting");
		shader.createUniform("useWaving");
		shader.createUniform("wavingValue");
		shader.createUniform("fogColor");
		
		shader.createUniform("numberOfRows");
		shader.createUniform("offset");
		
		shader.createUniform("plane");
		
		shader.setUniform("texture_sampler", 0, true);
		shader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
	}
	
	public void update(float dt) {
		temp += 0.5f * dt;
		waving = (float)Math.sin(temp) * 0.5f;
	}
	
	public void colorRender(List<GameObject> gameItems, Camera camera){
		OpenglUtils.cullFace(false);
		
		RenderEngine.clear(0, 0, 0);
		
		shader.bind();
		shader.setUniform("viewMatrix", MathUtils.getViewMatrix(camera));
		for(GameObject gameItem : gameItems){
			if(gameItem.isActive()) {
				Transform transform = gameItem.transform;
				
				if(camera.getFrusutmCuller().testEntityInView(gameItem)) {
					if(!gameItem.isFadeAway()) {
						MeshFilter meshFilter = gameItem.getComponent(MeshFilter.class);
						if(meshFilter != null) {
							if(meshFilter.enabled) {
								Mesh mesh = meshFilter.mesh;	
								
								shader.setUniform("modelMatrix", transform.getModelMatrix());	
								
								shader.setUniform("colorMode", 1);
								shader.setUniform("objectColor", MathUtils.getColorById(gameItem.getId()));
								
								GL30.glBindVertexArray(mesh.getVaoId());
								GL20.glEnableVertexAttribArray(0);		
								GL20.glEnableVertexAttribArray(1);
								GL20.glEnableVertexAttribArray(2);
								
								glDrawElements(GL_TRIANGLES, mesh.getVertexCount(), GL_UNSIGNED_INT, 0);
								
								GL20.glDisableVertexAttribArray(0);
								GL20.glDisableVertexAttribArray(1);
								GL20.glDisableVertexAttribArray(2);
								GL30.glBindVertexArray(0);
							}
						}
					}
				}
			}
		}
		shader.unbind();
		
		OpenglUtils.cullFace(true);
	}
	
	public void render(List<GameObject> gameItems, Light light, Color fogColor, Vector4f plane, Camera camera) {		
		shader.bind();
		
		if(Display.wasResized()) {
			shader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
		}
		
		shader.setUniform("viewMatrix", camera.getViewMatrix());
		shader.setUniform("lightPosition", light.getPosition());
		shader.setUniform("lightColor", light.getDiffuse());
		shader.setUniform("fogColor", fogColor);
		shader.setUniform("plane", plane);
		
		for(GameObject gameItem : gameItems){
			Transform transform = gameItem.transform;
			
			if(camera.getFrusutmCuller().testEntityInView(gameItem)) {
				if(!gameItem.isFadeAway()) {
					MeshFilter meshFilter = gameItem.getComponent(MeshFilter.class);
					if(meshFilter != null) {
						if(meshFilter.enabled) {
							shader.setUniform("modelMatrix", transform.getModelMatrix());
							
							shader.setUniform("useWaving", gameItem.isUseWaving());
							shader.setUniform("wavingValue", waving);
							
							Mesh mesh = meshFilter.mesh;
							Material material = mesh.getMaterial();
							
							shader.setUniform("numberOfRows", mesh.getMaterial().getNumberOfRows());
							shader.setUniform("offset", new Vector2f(gameItem.getTextureXOffset(), gameItem.getTextureYOffset()));
							
							GL13.glActiveTexture(GL13.GL_TEXTURE0);
							GL11.glBindTexture(GL11.GL_TEXTURE_2D, material.getTextureId());
							shader.setUniform("colorMode", 0);
							
							shader.setUniform("shineDamper", material.getShineDamper());
							shader.setUniform("reflectivity", material.getReflectivity());
							
							if(material.isHasTransparency()){
								OpenglUtils.cullFace(false);
							}
							
							shader.setUniform("useFakeLighting", material.isUseFakeLighting());
							
							GL30.glBindVertexArray(mesh.getVaoId());
							GL20.glEnableVertexAttribArray(0);		
							GL20.glEnableVertexAttribArray(1);
							GL20.glEnableVertexAttribArray(2);
							
							glDrawElements(GL_TRIANGLES, mesh.getVertexCount(), GL_UNSIGNED_INT, 0);
							
							GL20.glDisableVertexAttribArray(0);
							GL20.glDisableVertexAttribArray(1);
							GL20.glDisableVertexAttribArray(2);
							GL30.glBindVertexArray(0);
							
							GL11.glBindTexture(GL11.GL_TEXTURE_2D, 0);
							
							OpenglUtils.cullFace(true);
						}
					}
				}
			}
		}
		
		shader.unbind();
	}
	
	public void cleanup() {
		shader.cleanup();
	}
}
