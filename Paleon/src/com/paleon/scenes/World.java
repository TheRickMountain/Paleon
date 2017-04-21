package com.paleon.scenes;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.components.Image;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.RenderEngine;
import com.paleon.engine.graph.font.FontType;
import com.paleon.engine.graph.font.TextMeshData;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.Light;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.GrassRenderer;
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
	
	private final List<GameObject> meshRendererComponents = new ArrayList<GameObject>();
	private final List<GameObject> imageComponents = new ArrayList<GameObject>();
	private static Map<FontType, List<GameObject>> texts = new HashMap<FontType, List<GameObject>>();
	
	private WaterFrameBuffers fbos;
	private final List<WaterTile> waterTiles = new ArrayList<WaterTile>();
	
	private Skybox skybox;
	
	public Weather weather;
	
	public Light sun;
	
	private boolean wireframeMode = false;
	private boolean colorMode = false;
	
	private float waterHeight = -1;
	
	private int colorPickingId = -1;
	
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
		
		if(skybox != null) {
			skybox.update(dt);
		}
	}
	
	public void render(Camera camera, GrassRenderer grassRenderer) {
		if(Keyboard.isKeyDown(Key.F5)) {
			wireframeMode = !wireframeMode;
			OpenglUtils.wireframeMode(wireframeMode);
		}
		
		if(Keyboard.isKeyDown(Key.F6)) {
			colorMode = !colorMode;
		}
		
		RenderEngine.clear(0, 0, 0);
		renderEngine.renderColor(meshRendererComponents, camera);
		colorPickingId = ColorPicker.getId(Display.getWidth() / 2, Display.getHeight() / 2);
		
		GL11.glEnable(GL30.GL_CLIP_DISTANCE0);
		// Render reflection texture
		fbos.bindReflectionFrameBuffer();
		float distance = 2 * (camera.getPosition().y - waterHeight);
		camera.getPosition().y -= distance;
		camera.invertPitch();

		RenderEngine.clear(250f / 255f, 230f / 255f, 200f / 255f);
		renderEngine.render(meshRendererComponents, sun, weather.getFogColor(), new Vector4f(0, 1, 0, -waterHeight), camera);
		renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, 1, 0, -waterHeight), camera);
		if(skybox != null) {
			renderEngine.render(skybox, weather.getFogColor(), camera);
		}

		camera.getPosition().y += distance;
		camera.invertPitch();
		
		// Render refraction texture
		fbos.bindRefractionFrameBuffer();

		RenderEngine.clear(250f / 255f, 230f / 255f, 200f / 255f);
		renderEngine.render(meshRendererComponents, sun, weather.getFogColor(), new Vector4f(0, -1, 0, waterHeight), camera);
		renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, -1, 0, waterHeight), camera);
		if(skybox != null) {
			renderEngine.render(skybox, weather.getFogColor(), camera);
		}
		
		// Render to the world
		GL11.glDisable(GL30.GL_CLIP_DISTANCE0);
		fbos.unbindCurrentFrameBuffer();
		RenderEngine.clear(250f / 255f, 230f / 255f, 200f / 255f);
		
		renderEngine.render(meshRendererComponents, sun, weather.getFogColor(), new Vector4f(0, 1, 0, 100000), camera);
		renderEngine.render(terrains, sun, weather.getFogColor(), new Vector4f(0, 1, 0, 100000), camera);
		if(skybox != null) {
			renderEngine.render(skybox, weather.getFogColor(), camera);
		}
		renderEngine.render(waterTiles, sun, weather.getFogColor(), fbos, camera);
		grassRenderer.render(camera, sun, weather.getFogColor());
		
		renderEngine.renderGUI(imageComponents);
		
		renderEngine.renderText(texts);
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
		if(gameObject.getComponent(MeshRenderer.class) != null) {
			meshRendererComponents.add(gameObject);
		}

		if(gameObject.getComponent(Image.class) != null) {
			imageComponents.add(gameObject);
		}
		
		if(gameObject.getComponent(Text.class) != null) {
			Text text = gameObject.getComponent(Text.class);
			FontType font = text.font;
	        TextMeshData data = font.loadText(text);
	        Mesh mesh = new Mesh(data.getVertexPositions(), data.getTextureCoords());
	        text.textMeshVao = mesh.getVaoId();
	        text.vertexCount = data.getVertexCount();
	        List<GameObject> textBatch = texts.get(font);
	        if(textBatch == null){
	            textBatch = new ArrayList<GameObject>();
	            texts.put(font, textBatch);
	        }
	        textBatch.add(gameObject);
		}
	}
	
	public void addWaterTile(WaterTile waterTile) {
		waterTiles.add(waterTile);
	}
	
	public void setSkybox(Skybox skybox) {
		this.skybox = skybox;
	}
	
	public List<GameObject> getMeshRendererComponents() {
		return meshRendererComponents;
	}
	
	public List<GameObject> getImageComponents() {
		return imageComponents;
	}
     
    public void removeText(GameObject gameObject){
    	Text text = gameObject.getComponent(Text.class);
        List<GameObject> textBatch = texts.get(text.font);
        textBatch.remove(text);
        if(textBatch.isEmpty()){
            texts.remove(texts.get(text.font));
        }
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

}
