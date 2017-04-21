package com.paleon.engine.loaders;

import com.paleon.maths.vecmath.Vector3f;

public class Vertex {

	private static final int NO_INDEX = -1;

	private Vector3f position;
	public int textureIndex = NO_INDEX;
	public int normalIndex = NO_INDEX;
	public Vertex duplicateVertex = null;
	private int index;
	private float length;

	public Vertex(int index, Vector3f position) {
		this.index = index;
		this.position = position;
		this.length = position.length();
	}

	public int getIndex() {
		return index;
	}

	public float getLength() {
		return length;
	}

	public boolean isSet() {
		return textureIndex != NO_INDEX && normalIndex != NO_INDEX;
	}

	public boolean hasSameTextureAndNormal(int textureIndexOther, int normalIndexOther) {
		return textureIndexOther == textureIndex && normalIndexOther == normalIndex;
	}

	public Vector3f getPosition() {
		return position;
	}
	
}
