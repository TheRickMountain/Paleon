package com.paleon.engine.graph.renderer;

import com.paleon.engine.graph.shaders.ShaderProgram;
import com.paleon.engine.graph.shaders.UniformBoolean;
import com.paleon.engine.graph.shaders.UniformColor;
import com.paleon.engine.graph.shaders.UniformFloat;
import com.paleon.engine.graph.shaders.UniformInt;
import com.paleon.engine.graph.shaders.UniformMatrix;
import com.paleon.engine.graph.shaders.UniformSampler;
import com.paleon.engine.graph.shaders.UniformVec2;
import com.paleon.engine.graph.shaders.UniformVec3;
import com.paleon.engine.graph.shaders.UniformVec4;
import com.paleon.engine.toolbox.MyFile;

public class MeshShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/entity.vs");
	private static final MyFile FRAGMENT = new MyFile("shaders/entity.fs");
	
	public UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	
	public UniformVec3 lightPosition = new UniformVec3("lightPosition");
	public UniformInt numberOfRows = new UniformInt("numberOfRows");
	public UniformVec2 offset = new UniformVec2("offset");
	public UniformVec4 plane = new UniformVec4("plane");
	
	public UniformSampler texture_sampler = new UniformSampler("texture_sampler");
	public UniformColor lightColor = new UniformColor("lightColor");
	public UniformFloat shineDamper = new UniformFloat("shineDamper");
	public UniformFloat reflectivity = new UniformFloat("reflectivity");
	public UniformBoolean useFakeLighting = new UniformBoolean("useFakeLighting");
	public UniformColor fogColor = new UniformColor("fogColor");
	
	public MeshShader() {
		super(VERTEX, FRAGMENT, "position", "textureCoord", "normal");
		storeAllUniformLocations(modelMatrix, viewMatrix, projectionMatrix, lightPosition,
				numberOfRows, offset, plane, texture_sampler, lightColor, shineDamper, reflectivity,
				useFakeLighting, fogColor);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		texture_sampler.loadTexUnit(0);
		stop();
	}

}
