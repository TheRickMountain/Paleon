package com.paleon.textures;

import com.paleon.toolbox.MyFile;

public class TextureBuilder {

	private boolean clampEdges = false;
	private boolean mipmap = false;
	private boolean anisotropic = true;
	private boolean nearest = false;
	private float lodBiasValue = 0.0f;
	private int maxLevel = 0;
	
	private MyFile file;
	
	protected TextureBuilder(MyFile textureFile){
		this.file = textureFile;
	}
	
	public Texture create(){
		TextureData textureData = TextureUtils.decodeTextureFile(file);
		int textureId = TextureUtils.loadTextureToOpenGL(textureData, this);
		return new Texture(textureId, textureData.getWidth());
	}
	
	public TextureBuilder clampEdges(){
		this.clampEdges = true;
		return this;
	}
	
	public TextureBuilder normalMipMap(float lodBias){
		this.lodBiasValue = lodBias;
		this.mipmap = true;
		this.anisotropic = false;
		return this;
	}
	
	public TextureBuilder normalMipMap(){
		this.mipmap = true;
		this.anisotropic = false;
		return this;
	}
	
	public TextureBuilder nearestFiltering(){
		this.anisotropic = false;
		this.nearest = true;
		return this;
	}
	
	public TextureBuilder anisotropic(){
		this.mipmap = true;
		this.anisotropic = true;
		return this;
	}
	
	public TextureBuilder setMaxLevel(int maxLevel) {
		this.maxLevel = maxLevel;
		return this;
	}
	
	public int getMaxLevel() {
		return maxLevel;
	}
	
	protected boolean isClampEdges() {
		return clampEdges;
	}

	protected boolean isMipmap() {
		return mipmap;
	}

	protected boolean isAnisotropic() {
		return anisotropic;
	}

	protected boolean isNearest() {
		return nearest;
	}
	
	protected float getLodBias() {
		return lodBiasValue;
	}
	
}
