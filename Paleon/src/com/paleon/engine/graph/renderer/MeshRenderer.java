package com.paleon.engine.graph.renderer;

import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;

import java.util.List;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.Display;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Entity;
import com.paleon.engine.items.Light;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector4f;

public class MeshRenderer {

	private Camera camera;
	
	private MeshShader shader;
	
	public MeshRenderer(Camera camera) {
		this.camera = camera;
		shader = new MeshShader();
		
		shader.start();
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.stop();
	}
	
	public void render(List<Entity> entities, Light light, Color fogColor, Vector4f plane) {		
		shader.start();
		
		if(Display.wasResized()) {
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
		
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		shader.lightPosition.loadVec3(light.getPosition());
		shader.lightColor.loadColor(light.getDiffuse());
		shader.fogColor.loadColor(fogColor);
		shader.plane.loadVec4(plane);
		
		for(Entity entity : entities){
			Transform transform = entity.transform;
			
			if(camera.getFrusutmCuller().testEntityInView(entity)) {
				if(!entity.isFadeAway()) {
					shader.modelMatrix.loadMatrix(transform.getModelMatrix());
					
					Mesh mesh = entity.getMesh();
					Material material = entity.getMaterial();
					
					shader.numberOfRows.loadInt(material.getNumberOfRows());
					shader.offset.loadVec2(entity.getTextureXOffset(), entity.getTextureYOffset());
					
					material.texture.bindToUnit(0);
					
					shader.shineDamper.loadFloat(material.getShineDamper());
					shader.reflectivity.loadFloat(material.getReflectivity());
					
					if(material.isHasTransparency()){
						OpenglUtils.cullFace(false);
					}
					
					shader.useFakeLighting.loadBoolean(material.isUseFakeLighting());
					
					mesh.bind(0, 1, 2);
					
					glDrawElements(GL_TRIANGLES, mesh.getVertexCount(), GL_UNSIGNED_INT, 0);
					
					mesh.unbind(0, 1, 2);
					
					GL11.glBindTexture(GL11.GL_TEXTURE_2D, 0);
					
					OpenglUtils.cullFace(true);
				}
			}
		}
		
		shader.stop();
	}
	
	public void cleanup() {
		shader.cleanup();
	}
}
