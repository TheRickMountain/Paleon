package com.paleon.renderer;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;

import com.paleon.core.Camera;
import com.paleon.ecs.Entity;
import com.paleon.graph.Mesh;
import com.paleon.terrain.Terrain;
import com.paleon.terrain.TerrainBlock;

public class RenderEngine {

	private StaticRenderer staticRenderer;
	private TerrainRenderer terrainRenderer;
	
	public RenderEngine(Camera camera) {
		staticRenderer = new StaticRenderer(camera);
		terrainRenderer = new TerrainRenderer(camera);
	}
	
	private void clear() {
		GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);
	}
	
	public void render(Map<Mesh, List<Entity>> entities, Map<Terrain, List<TerrainBlock>> terrains) {
		clear();
		staticRenderer.render(entities);
		terrainRenderer.render(terrains);
	}
	
	public void cleanup() {
		staticRenderer.cleanup();
		terrainRenderer.cleanup();
	}
}
