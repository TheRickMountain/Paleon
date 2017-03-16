package com.paleon.renderer;

import com.paleon.core.Camera;
import com.paleon.terrain.Chunk;
import com.paleon.terrain.Terrain;

public class TerrainRenderer {

	private Camera camera;
	
	private TerrainShader shader;
	
	public TerrainRenderer(Camera camera) {
		this.camera = camera;
		shader = new TerrainShader();
	}
	
	public void render(Terrain terrain) {
		shader.start();
		
		shader.projectionMatrix.loadMatrix(camera.getProjectionMatrix());
		shader.viewMatrix.loadMatrix(camera.getViewMatrix());
		
		terrain.getAtlas().bindToUnit(0);
		
		for(Chunk chunk : terrain.getChunks()) {
			chunk.render();
		}
		shader.stop();
	}

	public void cleanup() {
		shader.cleanup();
	}
	
}
