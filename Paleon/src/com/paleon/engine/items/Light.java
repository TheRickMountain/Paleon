package com.paleon.engine.items;

import com.paleon.engine.toolbox.Color;
import com.paleon.maths.vecmath.Vector3f;

public class Light {

	private Vector3f position;

	private Color diffuse;
	
	public Light(Vector3f position, Color color) {
		this.position = position;
		this.diffuse = color;
	}
	
	public Vector3f getPosition() {
		return position;
	}
	
	public void setPosition(Vector3f position) {
		this.position = position;
	}
	
	public Color getDiffuse() {
		return diffuse;
	}
	
	public void setDiffuse(Color color) {
		this.diffuse = color;
	}

}
