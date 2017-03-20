package com.paleon.renderer;

import com.paleon.shaders.ShaderProgram;
import com.paleon.shaders.UniformColor;
import com.paleon.shaders.UniformInt;
import com.paleon.shaders.UniformMatrix;
import com.paleon.shaders.UniformSampler;
import com.paleon.utils.MyFile;

public class GUIShader extends ShaderProgram {
	
	private static final MyFile VERTEX_SHADER = new MyFile("shaders/gui.vert");
	private static final MyFile FRAGMENT_SHADER = new MyFile("shaders/gui.frag");
	
	protected UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	protected UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	
	private UniformSampler sprite = new UniformSampler("sprite");
	protected UniformColor color = new UniformColor("color");
	protected UniformInt hasTexture = new UniformInt("hasTexture");
	
	public GUIShader() {
		super(VERTEX_SHADER, FRAGMENT_SHADER, "in_data");
		storeAllUniformLocations(projectionMatrix, modelMatrix, sprite, color, hasTexture);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		sprite.loadTexUnit(0);
		stop();
	}
	
}
