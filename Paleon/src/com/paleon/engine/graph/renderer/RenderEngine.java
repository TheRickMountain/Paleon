package com.paleon.engine.graph.renderer;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Entity;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.water.WaterFrameBuffers;
import com.paleon.engine.weather.Skybox;
import com.paleon.maths.vecmath.Vector4f;

public class RenderEngine {
	
	public MeshRenderer meshRendererSystem;
	public TerrainRenderer terrainRendererSystem;
	public SkyboxRenderer skyboxRendererSystem;
	public WaterRenderer waterRendererSystem;
	
	private static RenderEngine instance;
	
	private RenderEngine(Camera camera) {	
		meshRendererSystem = new MeshRenderer(camera);
		terrainRendererSystem = new TerrainRenderer(camera);
		skyboxRendererSystem = new SkyboxRenderer(camera);
		waterRendererSystem = new WaterRenderer(camera);
		
		OpenglUtils.cullFace(true);
		GL11.glEnable(GL11.GL_DEPTH_TEST);
	}
	
	public static RenderEngine getInstance(Camera camera) {
		if(instance == null)
			instance = new RenderEngine(camera);
		
		return instance;
	}
	
	public void update(float deltaTime) {
		skyboxRendererSystem.update(deltaTime);
		waterRendererSystem.update(deltaTime);
	}
	
	public static void clear(float r, float g, float b) {
		GL11.glClearColor(r, g, b, 1.0f);
		GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);
	}
	
	public void render(List<Entity> gameObjects, Light light, Color fogColor, Vector4f plane) {
		meshRendererSystem.render(gameObjects, light, fogColor, plane);
	}
	
	public void render(Map<Terrain, List<TerrainBlock>> terrains, Light light, Color fogColor, Vector4f plane) {
		terrainRendererSystem.render(terrains, light, fogColor, plane);
	}
	
	public void render(Skybox skybox, Color fogColor) {
		skyboxRendererSystem.render(skybox, fogColor);
	}
	
	public void render(List<WaterTile> waters, Light light, Color fogColor, WaterFrameBuffers fbos) {
		waterRendererSystem.render(waters, light, fogColor, fbos);
	}

	public void cleanup(){
		meshRendererSystem.cleanup();
		terrainRendererSystem.cleanup();
		skyboxRendererSystem.cleanup();
		waterRendererSystem.cleanup();
	}
	
}
