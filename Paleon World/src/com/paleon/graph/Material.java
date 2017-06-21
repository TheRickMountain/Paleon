package com.paleon.graph;

import com.paleon.textures.Texture;
import com.paleon.toolbox.Color;

public class Material {
	
	private Texture texture;
	private Color color;
	private int numberOfRows = 1;
	private boolean hasTransparency = false;
	private boolean hasFakeLighting = false;
	
	public Material(Texture texture) {
		this(texture, new Color());
	}
	
	public Material(Texture texture, Color color) {
		this.texture = texture;
		this.color = color;
	}
	
	public Texture getTexture(){
		return texture;
	}
	
	public void setTexture(Texture texture) {
		this.texture = texture;
	}
	
	public Color getColor() {
		return color;
	}
	
	public int getNumberOfRows() {
		return numberOfRows;
	}

	public Material setNumberOfRows(int numberOfRows) {
		if(numberOfRows <= 0) {
            numberOfRows = 1;
		} else {
			this.numberOfRows = numberOfRows;
		}
		return this;
	}
	
	public void delete() {
		texture.cleanup();
	}

	public boolean isHasTransparency() {
		return hasTransparency;
	}

	public Material setHasTransparency(boolean hasTransparency) {
		this.hasTransparency = hasTransparency;
		return this;
	}

	public boolean isHasFakeLighting() {
		return hasFakeLighting;
	}

	public Material setHasFakeLighting(boolean hasFakeLighting) {
		this.hasFakeLighting = hasFakeLighting;
		return this;
	}
	
}
