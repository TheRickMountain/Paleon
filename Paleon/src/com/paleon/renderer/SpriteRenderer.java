package com.paleon.renderer;

import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.core.Camera;
import com.paleon.graph.Mesh;
import com.paleon.instances.Entity;
import com.paleon.math.Matrix4f;
import com.paleon.utils.MathUtils;

public class SpriteRenderer {

	private Camera camera;
	
	private SpriteShader shader;
	
	private Mesh mesh;
	
	private Matrix4f modelMatrix;
	
	public SpriteRenderer(Camera camera) {
		this.camera = camera;
		shader = new SpriteShader();
		setupMeshData();
		
		modelMatrix = new Matrix4f();
	}
	
	public void render(List<Entity> entities) {
		shader.start();
		
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		
		GL30.glBindVertexArray(mesh.getVao());
		GL20.glEnableVertexAttribArray(0);
		
		for(final Entity entity : entities) {
			shader.modelMatrix.loadMatrix(
					MathUtils.getModelMatrix(modelMatrix, 
							entity.getX(), entity.getY(), 0, entity.getScaleX(), entity.getScaleY()));
			shader.color.loadColor(entity.getColor());
			shader.hasTexture.loadInt(entity.isHasTexture() ? 1 : 0);
			
			if(entity.isHasTexture()) {
				entity.getTexture().bindToUnit(0);
			}
			
			GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, mesh.getVertexCount());
		}
		
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		
		shader.stop();
	}
	
	public void cleanup() {
		mesh.cleanup();
		shader.cleanup();
	}
	
	private void setupMeshData() {
		float[] data = new float[] {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
        };
	
		mesh = new Mesh(data, 2);
	}
	
}
