package com.paleon.engine.graph.renderer;

import com.paleon.engine.graph.shaders.ShaderProgram;
import com.paleon.engine.graph.shaders.UniformColor;
import com.paleon.engine.graph.shaders.UniformFloat;
import com.paleon.engine.graph.shaders.UniformMatrix;
import com.paleon.engine.graph.shaders.UniformSampler;
import com.paleon.engine.graph.shaders.UniformVec3;
import com.paleon.engine.toolbox.MyFile;

public class WaterShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/water.vs");
	private static final MyFile FRAGMENT = new MyFile("shaders/water.fs");
	
	public UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	public UniformVec3 cameraPosition = new UniformVec3("cameraPosition");
	public UniformVec3 lightPosition = new UniformVec3("lightPosition");
	
	private UniformSampler reflectionTexture = new UniformSampler("reflectionTexture");
	private UniformSampler refractionTexture = new UniformSampler("refractionTexture");
	private UniformSampler dudvMap = new UniformSampler("dudvMap");
	private UniformSampler normalMap = new UniformSampler("normalMap");
	private UniformSampler depthMap = new UniformSampler("depthMap");
	public UniformColor lightColor = new UniformColor("lightColor");
	public UniformColor fogColor = new UniformColor("fogColor");
	public UniformFloat moveFactor = new UniformFloat("moveFactor");
	
	public WaterShader() {
		super(VERTEX, FRAGMENT, "position");
		storeAllUniformLocations(modelMatrix, viewMatrix, projectionMatrix, cameraPosition,
				lightPosition, reflectionTexture, refractionTexture, dudvMap, normalMap,
				depthMap, lightColor, fogColor, moveFactor);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		reflectionTexture.loadTexUnit(0);
		refractionTexture.loadTexUnit(1);
		dudvMap.loadTexUnit(2);
		normalMap.loadTexUnit(3);
		depthMap.loadTexUnit(4);
		stop();
	}

}
