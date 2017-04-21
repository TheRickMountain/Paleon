package com.paleon.engine.terrain;

import java.nio.FloatBuffer;
import java.nio.IntBuffer;

import org.lwjgl.BufferUtils;
import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

public class Loader {

	public static int loadModelToVAO(float[] vertices, float[] normals, float[] textureCoords) {
		int vertexArrayID = createVAO();
		storeInterleavedDataInVAO(vertexArrayID, vertices, normals, textureCoords);
		return vertexArrayID;
	}
	
	public static int loadModelToVAO(float[] vertices, float[] textureCoords) {
		int vertexArrayID = createVAO();
		storeInterleavedDataInVAO(vertexArrayID, vertices, textureCoords);
		return vertexArrayID;
	}
	
	private static int createVAO() {
		int vertexArrayID = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vertexArrayID);
		return vertexArrayID;
	}
	
	private static void storeInterleavedDataInVAO(int vaoID, float[] vertices, float[] normals,
			float[] textureCoords) {
		FloatBuffer interleavedData = interleaveDataInBuffer(vertices, normals, textureCoords);
		int bufferObjectID = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, bufferObjectID);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, interleavedData, GL15.GL_STATIC_DRAW);

		int vertexByteCount = 4 * (3 + 3 + 2);
		GL20.glVertexAttribPointer(0, 3, GL11.GL_FLOAT, false, vertexByteCount, 0);
		GL20.glVertexAttribPointer(2, 3, GL11.GL_FLOAT, false, vertexByteCount,
				4 * 3);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, vertexByteCount,
				4 * 6);

		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
	}
	
	private static void storeInterleavedDataInVAO(int vaoID, float[] vertices, float[] textureCoords) {
		FloatBuffer interleavedData = interleaveDataInBuffer(vertices, textureCoords);
		int bufferObjectID = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, bufferObjectID);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, interleavedData, GL15.GL_STATIC_DRAW);

		int vertexByteCount = 4 * (3 + 2);
		GL20.glVertexAttribPointer(0, 3, GL11.GL_FLOAT, false, vertexByteCount, 0);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, vertexByteCount,
				4 * 3);

		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
	}
	
	private static FloatBuffer interleaveDataInBuffer(float[] vertices, float[] normals,
			float[] textureCoords) {
		FloatBuffer interleavedBuffer = BufferUtils.createFloatBuffer(vertices.length
				+ normals.length + textureCoords.length);
		int veticesPointer = 0;
		int normalsPointer = 0;
		int texturePointer = 0;
		for (int i = 0; i < vertices.length / 3; i++) {
			interleavedBuffer.put(new float[] { vertices[veticesPointer++],
					vertices[veticesPointer++], vertices[veticesPointer++] });
			interleavedBuffer.put(new float[] { normals[normalsPointer++],
					normals[normalsPointer++], normals[normalsPointer++] });
			interleavedBuffer.put(new float[] { textureCoords[texturePointer++],
					textureCoords[texturePointer++] });
		}
		interleavedBuffer.flip();
		return interleavedBuffer;
	}
	
	private static FloatBuffer interleaveDataInBuffer(float[] vertices, float[] textureCoords) {
		FloatBuffer interleavedBuffer = BufferUtils.createFloatBuffer(vertices.length
				+ textureCoords.length);
		int veticesPointer = 0;
		int texturePointer = 0;
		for (int i = 0; i < vertices.length / 3; i++) {
			interleavedBuffer.put(new float[] { vertices[veticesPointer++],
					vertices[veticesPointer++], vertices[veticesPointer++] });
			interleavedBuffer.put(new float[] { textureCoords[texturePointer++],
					textureCoords[texturePointer++] });
		}
		interleavedBuffer.flip();
		return interleavedBuffer;
	}
	
	public static int createIndicesVBO(int[] indices) {
		IntBuffer indicesBuffer = BufferUtils.createIntBuffer(indices.length);
		indicesBuffer.put(indices);
		indicesBuffer.flip();
		int indicesBufferId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, indicesBufferId);
		GL15.glBufferData(GL15.GL_ELEMENT_ARRAY_BUFFER, indicesBuffer, GL15.GL_STATIC_DRAW);
		return indicesBufferId;
	}
	
}
