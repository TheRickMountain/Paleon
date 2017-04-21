package com.paleon.scenes;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.graph.RenderEngine;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.ColorPicker;
import com.paleon.engine.toolbox.GameTime;
import com.paleon.engine.toolbox.OpenglUtils;
import com.paleon.engine.water.WaterFrameBuffers;
import com.paleon.engine.weather.Skybox;
import com.paleon.engine.weather.Weather;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class World {
	
	private RenderEngine renderEngine;
	
	public Map<Terrain, List<TerrainBlock>> terrains = new HashMap<Terrain, List<TerrainBlock>>();
	
	private static final int WORLD_TERRAIN_WIDTH = 3;
	private static final int WORLD_TILE_WIDTH = WORLD_TERRAIN_WIDTH * Terrain.TERRAIN_WIDTH_BLOCKS;
	
	private final TerrainBlock[][] terrainGrid;
	
	private final List<GameObject> gameObjects = new ArrayList<GameObject>();
	
	private WaterFrameBuffers fbos;
	private final List<WaterTile> waterTiles = new ArrayList<WaterTile>();
	
	private Skybox skybox;
	
	public Weather weather;
	
	public Light sun;
	
	private boolean wireframeMode = false;
	private boolean colorMode = false;
	
	private float waterHeight = -1;
	
	private int colorPickingId = -1;
	
	private GameObject colorPickedObject;
	
	public World(Camera camera) {
		fbos = new WaterFrameBuffers();
		renderEngine = RenderEngine.getInstance(camera);
		
		terrainGrid = new TerrainBlock[WORLD_TILE_WIDTH][WORLD_TILE_WIDTH];
		
		weather = new Weather();
		
		sun = new Light(new Vector3f(-200000, 2000000, 800000), new Color(250, 220, 190));
	}
	
	public void update(float dt) {		
		renderEngine.update(dt);
		
		weather.updateWeather(GameTime.getATime());
		sun.setDiffuse(weather.getSunLightColor());	
		skybox.update(dt);
		
		Iterator<GameObject> goIter = gameObjects.iterator();
		while(goIter.hasNext()){
			GameObject gameObject = goIter.next();
			gameObject.update(dt);
			
			if(colorPickingId != 0) {
				if(gameObject.getId() == colorPickingId) {
					colorPickedObject = gameObject;
				}
			} else {
				colorPickedObject = null;
			}
			
			if(gameObject.isRemove()) {
				goIter.remove();
			}
		}
		
		if(Keyboard.isKeyDown(Key.F5)) {
			wireframeMode = !wireframeMode;
			OpenglUtils.wireframeMode(wireframeMode);
		}
		
		if(Keyboard.isKeyDown(Key.F6)) {
			colorMode = !colorMode;
		}
	}
	
	public void render(Camera camera) {		
		if(Display.wasResized()) {
			camera.updateProjectionMatrix();
		}
		
		// Render in color mode
		RenderEngine.clear(0, 0, 0);
		renderEngine.renderColor(gameObjects, camera);
		colorPickingId = ColorPicker.getId(Display.getWidth() / 2, Display.getHeight() / 2);
		
		if(!colorMode) {
			GL11.glEnable(GL30.GL_CLIP_DISTANCE0);
			
			// Render reflection texture
			fbos.bindReflectionFrameBuffer();
			float distance = 2 * (camera.getPosition().y - waterHeight);
			camera.getPosition().y -= distance;
			camera.invertPitch();
	
			RenderEngine.clear(0.98f, 0.9f, 0.78f);
			renderEngine.render(gameObjects, sun, weather.getFogColor(), new Vector4f(0, 1, 0, -waterHeight), camera);
			renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, 1, 0, -waterHeight), camera);
			if(skybox != null) {
				renderEngine.render(skybox, weather.getFogColor(), camera);
			}
	
			camera.getPosition().y += distance;
			camera.invertPitch();
			
			// Render refraction texture
			fbos.bindRefractionFrameBuffer();
	
			RenderEngine.clear(0.98f, 0.9f, 0.78f);
			renderEngine.render(gameObjects, sun, weather.getFogColor(), new Vector4f(0, -1, 0, waterHeight), camera);
			renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, -1, 0, waterHeight), camera);
			if(skybox != null) {
				renderEngine.render(skybox, weather.getFogColor(), camera);
			}
			
			// Render to the world
			GL11.glDisable(GL30.GL_CLIP_DISTANCE0);
			fbos.unbindCurrentFrameBuffer();
			RenderEngine.clear(0.98f, 0.9f, 0.78f);
			
			renderEngine.render(gameObjects, sun, weather.getFogColor(), new Vector4f(0, 1, 0, 100000), camera);
			renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, 1, 0, 100000), camera);
			if(skybox != null) {
				renderEngine.render(skybox, weather.getFogColor(), camera);
			}
			renderEngine.render(waterTiles, sun, weather.getFogColor(), fbos, camera);
			
			// Render to the screen
			renderEngine.render(gameObjects);
		}
	}
	
	public void addTerrain(Terrain terrain){
		if(terrain.getGridX() > WORLD_TERRAIN_WIDTH || terrain.getGridZ() > WORLD_TERRAIN_WIDTH) {
			System.err.println("World not large enough to add terrain at " + terrain.getGridX());
			return;
		}
		
		List<TerrainBlock> terrainBlocks = new ArrayList<>();
		for(TerrainBlock terrainBlock : terrain.getTerrainBlocks()){
			int index = terrainBlock.getIndex();
			int gridX = (index % Terrain.TERRAIN_WIDTH_BLOCKS)
					+ (terrain.getGridX() * Terrain.TERRAIN_WIDTH_BLOCKS);
			int gridZ = (int) (Math.floor(index / Terrain.TERRAIN_WIDTH_BLOCKS) + (terrain
					.getGridZ() * Terrain.TERRAIN_WIDTH_BLOCKS));
			terrainGrid[gridX][gridZ] = terrainBlock;
			terrainBlocks.add(terrainBlock);
		}
		terrains.put(terrain, terrainBlocks);
	}
	
	public float getTerrainHeight(float x, float z){
		TerrainBlock block = getTerrainForPosition(x, z);
		if(block == null){
			return 0;
		}
		float terrainX = x - (block.getX() - Terrain.BLOCK_SIZE / 2);
		float terrainZ = z - (block.getZ() - Terrain.BLOCK_SIZE / 2);
		return block.calcHeight(terrainX, terrainZ);
	}
	
	private TerrainBlock getTerrainForPosition(float x, float z) {
		int terrain_i = (int) Math.floor(x / Terrain.BLOCK_SIZE);
		int terrain_j = (int) Math.floor(z / Terrain.BLOCK_SIZE);
		if (terrain_i < 0 || terrain_j < 0) {
			return null;
		}
		if (terrain_i >= terrainGrid.length) {
			return null;
		} else if (terrain_j >= terrainGrid[terrain_i].length) {
			return null;
		}
		return terrainGrid[terrain_i][terrain_j];
	}

	public void addGameObject(GameObject gameObject) {		
		for(GameObject child : gameObject.getChildren()){
			addGameObject(child);
		}
		
		gameObject.init();
		gameObjects.add(gameObject);
	}
	
	public void addGameObject(GameObject gameObject, boolean inverse) {		
		gameObject.init();
		gameObjects.add(gameObject);
		
		for(GameObject child : gameObject.getChildren()){
			addGameObject(child);
		}
	}
	
	public void removeGameObject(GameObject gameObject) {
		for(GameObject child : gameObject.getChildren()){
			removeGameObject(child);
		}
		
		gameObjects.remove(gameObject);
	}
	
	public void addWaterTile(WaterTile waterTile) {
		waterTiles.add(waterTile);
	}
	
	public void setSkybox(Skybox skybox) {
		this.skybox = skybox;
	}
	
	public int getColorPickingId(){
		return colorPickingId;
	}

	public void cleanup() {
		fbos.cleanup();
		
		renderEngine.cleanup();
		
		if(skybox != null) {
			skybox.cleanup();
		}
	}
	
	public GameObject getColorPickedObject() {
		return colorPickedObject;
	}

}
