package com.paleon.engine.terrain;

import com.paleon.maths.vecmath.Vector3f;

public class TerrainVertex {

	private float height;
	private Vector3f normal;

	protected TerrainVertex(float height, Vector3f normal) {
		this.height = height;
		this.normal = normal;
	}

	protected float getHeight() {
		return height;
	}

	protected Vector3f getNormal() {
		return normal;
	}

}
