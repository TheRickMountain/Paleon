package com.paleon.engine.graph.renderer;

import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;

import java.util.List;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Entity;
import com.paleon.engine.items.Light;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector4f;

public class MeshRenderer {

	private ShaderProgram shader;
	
	private float waving;
	private float temp = 0;
	
	public MeshRenderer(Camera camera) {
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
	
	public void render(List<Entity> gameItems, Light light, Color fogColor, Vector4f plane, Camera camera) {		
		shader.bind();
		
		if(Display.wasResized()) {
			shader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
		}
		
		shader.setUniform("viewMatrix", camera.getViewMatrix());
		shader.setUniform("lightPosition", light.getPosition());
		shader.setUniform("lightColor", light.getDiffuse());
		shader.setUniform("fogColor", fogColor);
		shader.setUniform("plane", plane);
		
		for(Entity gameItem : gameItems){
			Transform transform = gameItem.transform;
			
			if(camera.getFrusutmCuller().testEntityInView(gameItem)) {
				if(!gameItem.isFadeAway()) {
					shader.setUniform("modelMatrix", transform.getModelMatrix());
					
					shader.setUniform("useWaving", gameItem.isUseWaving());
					shader.setUniform("wavingValue", waving);
					
					Mesh mesh = gameItem.getMesh();
					Material material = gameItem.getMaterial();
					
					shader.setUniform("numberOfRows", material.getNumberOfRows());
					shader.setUniform("offset", new Vector2f(gameItem.getTextureXOffset(), gameItem.getTextureYOffset()));
					
					material.texture.bindToUnit(0);
					
					shader.setUniform("shineDamper", material.getShineDamper());
					shader.setUniform("reflectivity", material.getReflectivity());
					
					if(material.isHasTransparency()){
						OpenglUtils.cullFace(false);
					}
					
					shader.setUniform("useFakeLighting", material.isUseFakeLighting());
					
					mesh.bind(0, 1, 2);
					
					glDrawElements(GL_TRIANGLES, mesh.getVertexCount(), GL_UNSIGNED_INT, 0);
					
					mesh.unbind(0, 1, 2);
					
					GL11.glBindTexture(GL11.GL_TEXTURE_2D, 0);
					
					OpenglUtils.cullFace(true);
				}
			}
		}
		
		shader.unbind();
	}
	
	public void cleanup() {
		shader.cleanup();
	}
}
