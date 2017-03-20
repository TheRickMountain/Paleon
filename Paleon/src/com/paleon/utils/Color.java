package com.paleon.utils;

public class Color {
	
	public float r;
	public float g;
	public float b;
	public float a;
	
	public static final Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	
	public Color() {
		set(1.0f, 1.0f, 1.0f, 1.0f);
	}
	
	public Color(float r, float g, float b) {
		set(r, g, b);
	}
	
	public Color(float r, float g, float b, float a) {
		set(r, g, b, a);
	}
	
	public Color set(float r, float g, float b) {
		this.r = r;
		this.g = g;
		this.b = b;
		return this;
	}
	
	public Color set(float r, float g, float b, float a) {
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
		return this;
	}

	public float getR() {
		return r;
	}

	public void setR(float r) {
		this.r = r;
	}

	public float getG() {
		return g;
	}

	public void setG(float g) {
		this.g = g;
	}

	public float getB() {
		return b;
	}

	public void setB(float b) {
		this.b = b;
	}

	public float getA() {
		return a;
	}

	public void setA(float a) {
		this.a = a;
	}
	
	public Color convert() {
		this.r /= 255f;
		this.g /= 255f;
		this.b /= 255f;
		this.a /= 255f;
		return this;
	}

}
