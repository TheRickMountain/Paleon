package com.paleon.renderer;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.core.Display;
import com.paleon.core.GUITexture;
import com.paleon.graph.Mesh;
import com.paleon.math.Matrix4f;
import com.paleon.utils.Color;
import com.paleon.utils.MathUtils;

public class GUIRenderer {
	
	private GUIShader shader;
	
	private Mesh mesh;
	
	private Matrix4f modelMatrix;
	private Matrix4f projectionMatrix;
	
	public GUIRenderer() {
		shader = new GUIShader();
		setupMeshData();
		
		modelMatrix = new Matrix4f();
		projectionMatrix = new Matrix4f();
	}
	
	public void start() {
		shader.start();
		
		shader.projectionMatrix.loadMatrix(MathUtils.getOrthoProjectionMatrix(
				projectionMatrix, 0, Display.getWidth(), Display.getHeight(), 0));
		
		GL30.glBindVertexArray(mesh.getVao());
		GL20.glEnableVertexAttribArray(0);
	}
	
	public void finish() {
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
		
		shader.stop();
	}
	
	public void render(GUITexture entity) {
		shader.modelMatrix.loadMatrix(
				MathUtils.getModelMatrix(modelMatrix, 
						entity.rect.x, entity.rect.y, 0, entity.rect.width, entity.rect.height));
		shader.color.loadColor(Color.WHITE);
		shader.hasTexture.loadInt(1);

		entity.texture.bindToUnit(0);

		GL11.glDrawArrays(GL11.GL_TRIANGLE_STRIP, 0, mesh.getVertexCount());
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
