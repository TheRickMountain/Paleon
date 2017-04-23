package com.paleon.engine.graph.renderer;

import com.paleon.engine.graph.shaders.ShaderProgram;
import com.paleon.engine.graph.shaders.UniformColor;
import com.paleon.engine.graph.shaders.UniformFloat;
import com.paleon.engine.graph.shaders.UniformMatrix;
import com.paleon.engine.graph.shaders.UniformSampler;
import com.paleon.engine.toolbox.MyFile;

public class SkyboxShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/skybox.vs");
	private static final MyFile FRAGMENT = new MyFile("shaders/skybox.fs");
	
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformFloat blendFactor = new UniformFloat("blendFactor");
	public UniformColor fogColor = new UniformColor("fogColor");
	
	private UniformSampler samplerCube1 = new UniformSampler("sampler_cube_1");
	private UniformSampler samplerCube2 = new UniformSampler("sampler_cube_2");
	
	public SkyboxShader() {
		super(VERTEX, FRAGMENT, "position");
		storeAllUniformLocations(projectionMatrix, viewMatrix, blendFactor, fogColor);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		samplerCube1.loadTexUnit(0);
		samplerCube2.loadTexUnit(1);
		stop();
	}

}
