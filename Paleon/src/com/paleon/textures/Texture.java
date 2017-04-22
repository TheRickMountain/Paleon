package com.paleon.textures;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;

import com.paleon.engine.toolbox.MyFile;

public class Texture {

	public final int textureId;
	public final int size;
	private final int type;

	protected Texture(int textureId, int size) {
		this.textureId = textureId;
		this.size = size;
		this.type = GL11.GL_TEXTURE_2D;
	}

	protected Texture(int textureId, int type, int size) {
		this.textureId = textureId;
		this.size = size;
		this.type = type;
	}

	public void bindToUnit(int unit) {
		GL13.glActiveTexture(GL13.GL_TEXTURE0 + unit);
		GL11.glBindTexture(type, textureId);
	}

	public void cleanup() {
		GL11.glDeleteTextures(textureId);
	}

	public static TextureBuilder newTexture(MyFile textureFile) {
		return new TextureBuilder(textureFile);
	}

	public static Texture newCubeMap(MyFile[] textureFiles) {
		int cubeMapId = TextureUtils.loadCubeMap(textureFiles);
		//TODO needs to know size!
		return new Texture(cubeMapId, GL13.GL_TEXTURE_CUBE_MAP, 0);
	}
	
	public static Texture newEmptyCubeMap(int size) {
		int cubeMapId = TextureUtils.createEmptyCubeMap(size);
		return new Texture(cubeMapId, GL13.GL_TEXTURE_CUBE_MAP, size);
	}

}
