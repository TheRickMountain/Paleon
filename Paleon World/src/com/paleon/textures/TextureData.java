package com.paleon.textures;

import java.nio.ByteBuffer;

public class TextureData {
	
	private int width;
	private int height;
	private ByteBuffer buffer;
	
	public TextureData(int width, int height, ByteBuffer buffer) {
		this.width = width;
		this.height = height;
		this.buffer = buffer;
	}

	public int getWidth() {
		return width;
	}

	public int getHeight() {
		return height;
	}

	public ByteBuffer getBuffer() {
		return buffer;
	}

}
