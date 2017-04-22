package com.paleon.engine.toolbox;

import java.nio.FloatBuffer;
import java.nio.IntBuffer;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;
import org.lwjgl.system.MemoryUtil;

public class OpenglUtils {

	public static void cullFace(boolean enable) {
		if(enable){
			GL11.glEnable(GL11.GL_CULL_FACE);
			GL11.glCullFace(GL11.GL_BACK);
		} else {
			GL11.glDisable(GL11.GL_CULL_FACE);
		}
	}
	
	public static void alphaBlending(boolean enable) {
		if (enable) {
			GL11.glEnable(GL11.GL_BLEND);
			GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
		} else {
			GL11.glDisable(GL11.GL_BLEND);
		}
	}
	
	public static void depthTest(boolean enable) {
		if(enable) {
			GL11.glEnable(GL11.GL_DEPTH_TEST);
		} else {
			GL11.glDisable(GL11.GL_DEPTH_TEST);
		}
	}
	
	public static void wireframeMode(boolean enable) {
		if(enable) {
			GL11.glPolygonMode(GL11.GL_FRONT_AND_BACK, GL11.GL_LINE);
		} else {
			GL11.glPolygonMode(GL11.GL_FRONT_AND_BACK, GL11.GL_FILL);
		}
	}
	
	public static FloatBuffer toFloatBuffer(float[] data) {
		FloatBuffer buffer = MemoryUtil.memAllocFloat(data.length);
		buffer.put(data).flip();
		return buffer;
	}
	
	public static IntBuffer toIntBuffer(int[] data) {
		IntBuffer buffer = MemoryUtil.memAllocInt(data.length);
		buffer.put(data).flip();
		return buffer;
	}
	
	public static FloatBuffer toFloatBuffer(float[] vertices, float[] texCoords) {
        FloatBuffer interleavedBuffer = MemoryUtil.memAllocFloat(vertices.length + texCoords.length);
        int veticesPointer = 0;
        int texturePointer = 0;
        for (int i = 0; i < vertices.length / 2; i++) {
            interleavedBuffer.put(new float[] { vertices[veticesPointer++],
                    vertices[veticesPointer++] });
            interleavedBuffer.put(new float[] { texCoords[texturePointer++],
                    texCoords[texturePointer++] });
        }
        interleavedBuffer.flip();
        return interleavedBuffer;
    }
	
	public static FloatBuffer toFloatBuffer(float[] vertices, float[] texCoords, float[] normals) {
        FloatBuffer interleavedBuffer = MemoryUtil.memAllocFloat(vertices.length + texCoords.length + normals.length);
        int veticesPointer = 0;
        int texturePointer = 0;
        int normalsPointer = 0;
        for (int i = 0; i < vertices.length / 3; i++) {
            interleavedBuffer.put(new float[] { vertices[veticesPointer++],
                    vertices[veticesPointer++], vertices[veticesPointer++] });
            interleavedBuffer.put(new float[] { texCoords[texturePointer++],
                    texCoords[texturePointer++] });
            interleavedBuffer.put(new float[] { normals[normalsPointer++],
                    normals[normalsPointer++], normals[normalsPointer++] });
        }
        interleavedBuffer.flip();
        return interleavedBuffer;
    }
	
	public static void bindVAO(int vaoID) {
		GL30.glBindVertexArray(vaoID);
		GL20.glEnableVertexAttribArray(0);
		GL20.glEnableVertexAttribArray(1);
		GL20.glEnableVertexAttribArray(2);
	}
	
	public static void bindIndicesVBO(int vboID) {
		GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, vboID);
	}
	
	public static void unbindIndicesVBO() {
		GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, 0);
	}
	
	public static void unbindVAO() {
		GL20.glDisableVertexAttribArray(2);
		GL20.glDisableVertexAttribArray(1);
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
	}
	
}
