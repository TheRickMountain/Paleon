package com.paleon.renderer;

import com.paleon.shaders.ShaderProgram;
import com.paleon.shaders.UniformMatrix;
import com.paleon.toolbox.MyFile;

public class TerrainShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/terrain.vert");
	private static final MyFile FRAGMENT = new MyFile("shaders/terrain.frag");
	
	public UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	public UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	public UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	
	public TerrainShader() {
		super(VERTEX, FRAGMENT, "position", "textureCoords", "normal");
		storeAllUniformLocations(projectionMatrix, viewMatrix, modelMatrix);
	}

	@Override
	protected void connectTextureUnits() {
		
	}

}
