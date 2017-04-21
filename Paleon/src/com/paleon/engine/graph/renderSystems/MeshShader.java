package com.paleon.engine.graph.renderSystems;

import com.paleon.engine.graph.shaders.ShaderProgram;
import com.paleon.engine.graph.shaders.UniformColor;
import com.paleon.engine.graph.shaders.UniformMatrix;
import com.paleon.engine.toolbox.MyFile;

public class MeshShader extends ShaderProgram {

	private static final MyFile VERTEX = new MyFile("shaders/entity.vs");
	private static final MyFile FRAGMENT = new MyFile("shaders/entity.fs");
	
	protected UniformMatrix modelMatrix = new UniformMatrix("modelMatrix");
	protected UniformMatrix viewMatrix = new UniformMatrix("viewMatrix");
	protected UniformMatrix projectionMatrix = new UniformMatrix("projectionMatrix");
	
	protected UniformColor objectColor = new UniformColor("objectColor");
	
	
	public MeshShader() {
		super(VERTEX, FRAGMENT, "position", "textureCoord", "normal");
	}

	@Override
	protected void connectTextureUnits() {
		
	}

}
