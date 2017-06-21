package com.paleon.renderer;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.core.Camera;
import com.paleon.core.Display;
import com.paleon.ecs.Entity;
import com.paleon.graph.Material;
import com.paleon.graph.Mesh;
import com.paleon.math.Matrix4f;
import com.paleon.toolbox.MathUtils;

public class StaticRenderer {

	private Camera camera;
	
	private StaticShader shader;
	
	private Matrix4f modelMatrix = new Matrix4f();
	
	public StaticRenderer(Camera camera) {
		this.camera = camera;
		shader = new StaticShader();
		
		shader.start();
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.stop();
	}
	
	public void render(Map<Mesh, List<Entity>> entities) {
		prepare();
		for(Mesh mesh : entities.keySet()) {
			GL30.glBindVertexArray(mesh.getVAO());
			GL20.glEnableVertexAttribArray(0);
			GL20.glEnableVertexAttribArray(1);
			GL20.glEnableVertexAttribArray(2);
			
			List<Entity> batch = entities.get(mesh);
			for(Entity entity : batch) {
				render(entity);
			}
			
			GL30.glBindVertexArray(0);
			GL20.glDisableVertexAttribArray(0);
			GL20.glDisableVertexAttribArray(1);
			GL20.glDisableVertexAttribArray(2);
		}
		finish();
	}
	
	private void render(Entity entity) {
		Material material = entity.getMaterial();
		material.getTexture().bind(0);
		shader.modelMatrix.loadMatrix(MathUtils.getModelMatrix(modelMatrix, entity.getTransform()));
		GL11.glDrawElements(GL11.GL_TRIANGLES, entity.getMesh().getVertexCount(), GL11.GL_UNSIGNED_INT, 0);
	}
	
	private void prepare() {
		shader.start();
		
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		
		if(Display.isResized()) {
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
	}
	
	private void finish() {
		shader.stop();
	}
	
	public void cleanup() {
		shader.cleanup();
	}
	
}
