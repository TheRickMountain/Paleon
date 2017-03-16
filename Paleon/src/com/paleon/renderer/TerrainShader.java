package com.paleon.renderer;

import com.paleon.shaders.ShaderProgram;
import com.paleon.shaders.UniformMatrix;
import com.paleon.shaders.UniformSampler;
import com.paleon.utils.MyFile;

public class TerrainShader extends ShaderProgram {

	private static final MyFile VERTEX_SHADER = new MyFile("shaders/terrain.vert");
	private static final MyFile FRAGMENT_SHADER = new MyFile("shaders/terrain.frag");
	
	protected UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	protected UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	
	private UniformSampler atlas = new UniformSampler("atlas");
	
	public TerrainShader() {
		super(VERTEX_SHADER, FRAGMENT_SHADER, "in_position", "in_texCoords");
		storeAllUniformLocations(projectionMatrix, viewMatrix, atlas);
		connectTextureUnits();
	}

	@Override
	protected void connectTextureUnits() {
		start();
		atlas.loadTexUnit(0);
		stop();
	}

}
