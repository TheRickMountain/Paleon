package com.paleon.engine.graph.renderer;

import com.paleon.engine.Display;
import com.paleon.engine.items.Camera;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.weather.Skybox;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;

public class SkyboxRenderer {

	private Camera camera;
	
	private SkyboxShader shader;
	
	private static final float ROTATE_SPEED = 0.5f;
	private float rotation = 0;
	
	public SkyboxRenderer(Camera camera) {
		this.camera = camera;
		shader = new SkyboxShader();
		
		shader.start();
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.stop();
	}
	
	public void update(float dt) {
		rotation += ROTATE_SPEED * dt;
	}
	
	public void render(Skybox skybox, Color fogColor) {
		shader.start();
		
		if(Display.wasResized()){
			shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		}
		
		Matrix4f viewMatrix = camera.getViewMatrix();
		viewMatrix.m30 = 0;
		viewMatrix.m31 = 0;
		viewMatrix.m32 = 0;
		Matrix4f.rotate((float)Math.toRadians(rotation), new Vector3f(0, 1, 0), viewMatrix, viewMatrix);
		shader.viewMatrix.loadMatrix(viewMatrix);
		shader.fogColor.loadColor(fogColor);
		skybox.render(shader);
		shader.stop();
	}
	
	public void cleanup() {
		shader.cleanup();
	}
	
}
