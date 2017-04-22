package com.paleon.engine.terrain;

import com.paleon.engine.ResourceManager;
import com.paleon.textures.Texture;

public class TexturePack {

	Texture blendMap;
	Texture aTexture;
	Texture rTexture;
	Texture gTexture;
	Texture bTexture;
	
	public TexturePack(String blendMap, String alpha, String red, String green, String blue) {
		this.blendMap = ResourceManager.getTexture(blendMap);
		this.aTexture = ResourceManager.getTexture(alpha);
		this.rTexture = ResourceManager.getTexture(red);
		this.gTexture = ResourceManager.getTexture(green);
		this.bTexture = ResourceManager.getTexture(blue);
	}

	public Texture getBlendMap() {
		return blendMap;
	}

	public Texture getaTexture() {
		return aTexture;
	}

	public Texture getrTexture() {
		return rTexture;
	}

	public Texture getgTexture() {
		return gTexture;
	}

	public Texture getbTexture() {
		return bTexture;
	}
	
}
