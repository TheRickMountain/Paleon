package com.paleon.engine.terrain;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;
import org.lwjgl.opengl.GL31;
import org.lwjgl.opengl.GL33;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Light;
import com.paleon.engine.loaders.TextureLoader;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;

public class GrassRenderer {

	private ShaderProgram shader;
	
	private int vaoId;
	
	private float[] offset;
	
	private float[] vertices;
	
	private int textureId;
	
	private float scale = 1.75f;
	
	public void init(Matrix4f projectionMatrix, float[] offset) {
		this.offset = offset;
		
		initShader(projectionMatrix);
		vertices = new float[]{
				0, 2, -1,
				0, 0, 1, 
				0, 0, -1,
				
				0, 2, -1,
				0, 0, 1,
				0, 2, 1,
				
				1, 2, 0,
				-1, 0, 0,
				1, 0, 0,
				
				1, 2, 0,
				-1, 0, 0,
				-1, 2, 0,
				
				0.707107f, 2, 0.707107f,
				-0.707107f, 0, -0.707107f,
				0.707107f, 0, 0.707107f,
				
				0.707107f, 2, 0.707107f,
				-0.707107f, 0, -0.707107f,
				-0.707107f, 2, -0.707107f,
				
				-0.707107f, 2, 0.707107f,
				0.707107f, 0, -0.707107f,
				-0.707107f, 0, 0.707107f,
				
				-0.707107f, 2, 0.707107f,
				0.707107f, 0, -0.707107f,
				0.707107f, 2, -0.707107f
		};
		
		for(int i = 0; i < vertices.length; i++) {
			vertices[i] *= scale;
		}
		
		float[] textureCoords = {
				0, 0,
				1, 1,
				0, 1,	
				0, 0,
				1, 1,
				1, 0,
				
				0, 0,
				1, 1,
				0, 1,
				0, 0,
				1, 1,
				1, 0,
				
				0, 0,
				1, 1,
				0, 1,
				0, 0,
				1, 1,
				1, 0,
				
				0, 0,
				1, 1,
				0, 1,
				0, 0,
				1, 1,
				1, 0,
		};
		
		int instanceVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, instanceVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, MathUtils.dataToFloatBuffer(offset), GL15.GL_STATIC_DRAW);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		
		vaoId = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vaoId);
		
		int vboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, MathUtils.dataToFloatBuffer(vertices), GL15.GL_STATIC_DRAW);
		GL20.glEnableVertexAttribArray(0);
		GL20.glVertexAttribPointer(0, 3, GL11.GL_FLOAT, false, 0, 0);
		
		vboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, MathUtils.dataToFloatBuffer(textureCoords), GL15.GL_STATIC_DRAW);
		GL20.glEnableVertexAttribArray(1);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, 0, 0);
		
		GL20.glEnableVertexAttribArray(2);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, instanceVboId);
		GL20.glVertexAttribPointer(2, 3, GL11.GL_FLOAT, false, 0, 0);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL33.glVertexAttribDivisor(2, 1);
		
		textureId = TextureLoader.load("/grass/grasstxt1.png");
	}
	
	private void initShader(Matrix4f projectionMatrix) {
		shader = ResourceManager.loadShader("grass");
		
		shader.createUniform("projectionMatrix");
		shader.createUniform("viewMatrix");
		shader.createUniform("modelMatrix");
		shader.createUniform("texture_sampler");
		shader.createUniform("lightPosition");
		shader.createUniform("lightColor");
		shader.createUniform("fogColor");
		
		shader.setUniform("projectionMatrix", projectionMatrix, true);
		shader.setUniform("modelMatrix", MathUtils.getModelMatrix(new Vector3f(0, 0, 0), 0, 0, 0, 1), true);
		shader.setUniform("texture_sampler", 0, true);
	}
	
	public void render(Camera camera, Light light, Color fogColor) {
		shader.bind();
		shader.setUniform("viewMatrix", MathUtils.getViewMatrix(camera));
		
		shader.setUniform("lightPosition", light.getPosition());
		shader.setUniform("lightColor", light.getDiffuse());
		shader.setUniform("fogColor", fogColor);
		
		GL13.glActiveTexture(GL13.GL_TEXTURE0);
		GL11.glBindTexture(GL11.GL_TEXTURE_2D, textureId);
		
		GL30.glBindVertexArray(vaoId);
		OpenglUtils.cullFace(false);
		GL31.glDrawArraysInstanced(GL11.GL_TRIANGLES, 0, vertices.length / 3, offset.length / 3);
		OpenglUtils.cullFace(true);
		GL30.glBindVertexArray(0);
		shader.unbind();
	}
	
	public void cleanup() {
		
	}
	
}
