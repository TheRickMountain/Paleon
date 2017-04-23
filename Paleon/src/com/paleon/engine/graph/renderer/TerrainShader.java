package com.paleon.engine.graph.renderer;

import com.paleon.engine.graph.shaders.ShaderProgram;
import com.paleon.engine.graph.shaders.UniformColor;
import com.paleon.engine.graph.shaders.UniformMatrix;
import com.paleon.engine.graph.shaders.UniformSampler;
import com.paleon.engine.graph.shaders.UniformVec3;
import com.paleon.engine.graph.shaders.UniformVec4;
import com.paleon.engine.toolbox.MyFile;

public class TerrainShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/terrain.vs");
	private static final MyFile FRAGMENT = new MyFile("shaders/terrain.fs");
	
	public UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	
	private UniformSampler blendMap = new UniformSampler("blendMap");
	private UniformSampler aTexture = new UniformSampler("aTexture");
	private UniformSampler rTexture = new UniformSampler("rTexture");
	private UniformSampler gTexture = new UniformSampler("gTexture");
	private UniformSampler bTexture = new UniformSampler("bTexture");
	
	public UniformVec3 lightPosition = new UniformVec3("lightPosition");
	public UniformColor lightColor = new UniformColor("lightColor");
	public UniformColor fogColor = new UniformColor("fogColor");
	
	public UniformVec4 plane = new UniformVec4("plane");
	
	
	public TerrainShader() {
		super(VERTEX, FRAGMENT, "position", "textureCoord", "normal");
		storeAllUniformLocations(modelMatrix, viewMatrix, projectionMatrix,
				blendMap, aTexture, rTexture, gTexture, bTexture, lightPosition,
				lightColor, fogColor);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		blendMap.loadTexUnit(0);
		aTexture.loadTexUnit(1);
		rTexture.loadTexUnit(2);
		gTexture.loadTexUnit(3);
		bTexture.loadTexUnit(4);
		stop();
	}

}
