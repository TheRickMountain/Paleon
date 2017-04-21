package com.paleon.engine.graph;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.components.Text;
import com.paleon.engine.graph.renderSystems.GUIRendererSystem;
import com.paleon.engine.graph.renderSystems.MeshRendererSystem;
import com.paleon.engine.graph.renderSystems.SkyboxRendererSystem;
import com.paleon.engine.graph.renderSystems.TerrainRendererSystem;
import com.paleon.engine.graph.renderSystems.WaterRendererSystem;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.toolbox.Rect;
import com.paleon.engine.water.WaterFrameBuffers;
import com.paleon.engine.weather.Skybox;
import com.paleon.maths.vecmath.Vector4f;

public class RenderEngine {
	
	public MeshRendererSystem meshRendererSystem;
	public TerrainRendererSystem terrainRendererSystem;
	public SkyboxRendererSystem skyboxRendererSystem;
	public WaterRendererSystem waterRendererSystem;
	public static GUIRendererSystem guiRendererSystem;
	
	private static RenderEngine instance;
	
	private RenderEngine(Camera camera) {	
		meshRendererSystem = new MeshRendererSystem(camera);
		terrainRendererSystem = new TerrainRendererSystem(camera);
		skyboxRendererSystem = new SkyboxRendererSystem(camera);
		waterRendererSystem = new WaterRendererSystem(camera);
		guiRendererSystem = new GUIRendererSystem();
		
		OpenglUtils.cullFace(true);
		GL11.glEnable(GL11.GL_DEPTH_TEST);
	}
	
	public static RenderEngine getInstance(Camera camera) {
		if(instance == null)
			instance = new RenderEngine(camera);
		
		return instance;
	}
	
	public void update(float deltaTime) {
		meshRendererSystem.update(deltaTime);
		skyboxRendererSystem.update(deltaTime);
		waterRendererSystem.update(deltaTime);
	}
	
	public static void clear(float r, float g, float b) {
		GL11.glClearColor(r, g, b, 1.0f);
		GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);
	}
	
	public void renderColor(List<GameObject> gameObjects, Camera camera) {
		meshRendererSystem.colorRender(gameObjects, camera);
	}
	
	public void render(List<GameObject> gameObjects, Light light, Color fogColor, Vector4f plane, Camera camera) {
		meshRendererSystem.render(gameObjects, light, fogColor, plane, camera);
	}
	
	public void render(Map<Terrain, List<TerrainBlock>> terrains, Light light, Color fogColor, Vector4f plane, Camera camera) {
		terrainRendererSystem.render(terrains, light, fogColor, plane, camera);
	}
	
	public void render(Skybox skybox, Color fogColor, Camera camera) {
		skyboxRendererSystem.render(skybox, fogColor, camera);
	}
	
	public void render(List<WaterTile> waters, Light light, Color fogColor, WaterFrameBuffers fbos, Camera camera) {
		waterRendererSystem.render(waters, camera, light, fogColor, fbos);
	}
	
	public void render(List<GameObject> gameObjects) {
		guiRendererSystem.render(gameObjects);
	}
	
	public static void renderGUI(Rect rect, int textureId) {
		guiRendererSystem.render(rect, textureId);
	}
	
	public static void renderGUI(Rect rect, Text text) {
		guiRendererSystem.render(rect, text);
	}

	public void cleanup(){
		meshRendererSystem.cleanup();
		terrainRendererSystem.cleanup();
		skyboxRendererSystem.cleanup();
		waterRendererSystem.cleanup();
		guiRendererSystem.cleanup();
	}
	
}
