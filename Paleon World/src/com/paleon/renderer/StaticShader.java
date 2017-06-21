package com.paleon.renderer;

import com.paleon.shaders.ShaderProgram;
import com.paleon.shaders.UniformMatrix;
import com.paleon.shaders.UniformSampler;
import com.paleon.toolbox.MyFile;

public class StaticShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/static.vert");
	private static final MyFile FRAGMENT = new MyFile("shaders/static.frag");
	
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	private UniformSampler diffuse = new UniformSampler("diffuse");
	
	public StaticShader() {
		super(VERTEX, FRAGMENT, "position", "textureCoords", "normal");
		storeAllUniformLocations(projectionMatrix, viewMatrix, modelMatrix);
	}

	@Override
	protected void connectTextureUnits() {
		start();
		diffuse.loadTexUnit(0);
		stop();
	}

}
