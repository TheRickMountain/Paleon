package com.paleon.engine.items;

public class WaterTile {

	public static final float TILE_SIZE = 60;
	
	private float x, height, z;

	public WaterTile(float centerX, float height, float centerZ) {
		this.x = centerX;
		this.height = height;
		this.z = centerZ;
	}

	public float getX() {
		return x;
	}

	public float getHeight() {
		return height;
	}

	public float getZ() {
		return z;
	}
	
}
