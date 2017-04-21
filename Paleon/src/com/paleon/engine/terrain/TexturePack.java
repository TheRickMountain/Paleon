package com.paleon.engine.terrain;

import com.paleon.engine.ResourceManager;

public class TexturePack {

	int blendMap;
	int aTexture;
	int rTexture;
	int gTexture;
	int bTexture;
	
	public TexturePack(String blendMap, String alpha, String red, String green, String blue) {
		this.blendMap = ResourceManager.getTexture(blendMap);
		this.aTexture = ResourceManager.getTexture(alpha);
		this.rTexture = ResourceManager.getTexture(red);
		this.gTexture = ResourceManager.getTexture(green);
		this.bTexture = ResourceManager.getTexture(blue);
	}

	public int getBlendMap() {
		return blendMap;
	}

	public int getaTexture() {
		return aTexture;
	}

	public int getrTexture() {
		return rTexture;
	}

	public int getgTexture() {
		return gTexture;
	}

	public int getbTexture() {
		return bTexture;
	}
	
}
