package com.paleon.engine.graph.renderSystems;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.items.Camera;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.weather.Skybox;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;

public class SkyboxRendererSystem {

	private ShaderProgram skyboxShader;
	
	private static final float ROTATE_SPEED = 0.5f;
	private float rotation = 0;
	
	public SkyboxRendererSystem(Camera camera) {
		skyboxShader = ResourceManager.loadShader("skybox");
		
		skyboxShader.createUniform("viewMatrix");
		skyboxShader.createUniform("projectionMatrix");
		skyboxShader.createUniform("sampler_cube_1");
		skyboxShader.createUniform("sampler_cube_2");
		skyboxShader.createUniform("blendFactor");
		skyboxShader.createUniform("fogColor");
		
		skyboxShader.setUniform("sampler_cube_1", 0, true);
		skyboxShader.setUniform("sampler_cube_2", 1, true);
		skyboxShader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
	}
	
	public void update(float dt) {
		rotation += ROTATE_SPEED * dt;
	}
	
	public void render(Skybox skybox, Color fogColor, Camera camera) {
		if(Display.wasResized()){
			skyboxShader.setUniform("projectionMatrix", camera.getProjectionMatrix(), true);
		}
		
		skyboxShader.bind();
		Matrix4f viewMatrix = camera.getViewMatrix();
		viewMatrix.m30 = 0;
		viewMatrix.m31 = 0;
		viewMatrix.m32 = 0;
		Matrix4f.rotate((float)Math.toRadians(rotation), new Vector3f(0, 1, 0), viewMatrix, viewMatrix);
		skyboxShader.setUniform("viewMatrix", viewMatrix);
		skyboxShader.setUniform("fogColor", fogColor);
		skybox.render(skyboxShader);
		skyboxShader.unbind();
	}
	
	public void cleanup() {
		skyboxShader.cleanup();
	}
	
}
