package com.paleon.engine.toolbox;

import com.paleon.maths.vecmath.Vector2f;

public class Rect {

	public float x;
	public float y;
	public float width;
	public float height;
	
	public Rect() {
		this(0, 0, 0, 0);
	}
	
	public Rect(Rect rect) {
		this(rect.x, rect.y, rect.width, rect.height);
	}
	
	public Rect(Vector2f position, Vector2f size) {
		this(position.x, position.y, size.x, size.y);
	}
	
	public Rect(float x, float y, float width, float height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}
	
}
