package com.paleon.engine.graph.shaders;

import org.lwjgl.opengl.GL20;

import com.paleon.engine.toolbox.Color;

public class UniformColor extends Uniform {
	private float currentR;
	private float currentG;
	private float currentB;
	private float currentA;
	private boolean used = false;

	public UniformColor(String name) {
		super(name);
	}

	public void loadColor(Color color) {
		loadColor(color.getRf(), color.getGf(), color.getBf(), color.getAf());
	}

	public void loadColor(float r, float g, float b, float a) {
		if (!used || r != currentR || g != currentG || b != currentB || a != currentA) {
			this.currentR = r;
			this.currentG = g;
			this.currentB = b;
			this.currentA = a;
			used = true;
			GL20.glUniform4f(super.getLocation(), r, g, b, a);
		}
	}

}
