package com.paleon.core;

import java.util.ArrayList;
import java.util.List;

import org.lwjgl.opengl.GL11;

import com.paleon.astar.PathTileGraph;
import com.paleon.instances.Entity;
import com.paleon.renderer.GUIRenderer;
import com.paleon.renderer.SpriteRenderer;
import com.paleon.renderer.TerrainRenderer;
import com.paleon.terrain.Terrain;
import com.paleon.terrain.Tile;
import com.paleon.utils.MousePicker;

public class World {
	
	private static final World INSTANCE = new World();
	
	public Camera camera;
	
	private TerrainRenderer terrainRenderer;
	private SpriteRenderer spriteRenderer;
	private GUIRenderer guiRenderer;
	
	private List<Entity> entities = new ArrayList<>();
	private List<Entity> entitiesToRemove = new ArrayList<>();
	private List<Entity> entitiesToAdd = new ArrayList<>();
	
	private int width = 2;
	private int height = 2;
	
	private Terrain terrain;
	
	private Tile[][] tiles;
	
	private Game game;
	private GUI gui;
	
	private PathTileGraph tileGraph;
	
	public List<Job> jobList = new ArrayList<>();
	
	private World() {}
	
	public static World getInstance() {
		return INSTANCE;
	}
	
	public void init() {
		GL11.glEnable(GL11.GL_BLEND);
		GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
		
		camera = new Camera();
		
		MousePicker.init(camera);
		
		terrainRenderer = new TerrainRenderer(camera);
		spriteRenderer = new SpriteRenderer(camera);
		guiRenderer = new GUIRenderer();
		
		terrain = new Terrain(width, height);
		tiles = new Tile[getWidth()][getHeight()];
		for(int i = 0; i < getWidth(); i++) {
			for(int j = 0; j < getHeight(); j++) {
				tiles[i][j] = terrain.getTile(i, j);
			}
		}
		
		game = new Game();
		gui = new GUI(guiRenderer);
	}
	
	public void update(float dt) {
		MousePicker.update();
		camera.update(dt);
		gui.update(dt);
		game.update(dt);
		
		if(!entitiesToAdd.isEmpty()) {
			entities.addAll(entitiesToAdd);
			entitiesToAdd.clear();
		}
		
		for(Entity entity : entities) {
			entity.update(dt);
			
			if(entity.isRemoved()) {
				entitiesToRemove.add(entity);
			}
		}
		
		if(!entitiesToRemove.isEmpty()) {
			entities.removeAll(entitiesToRemove);
			entitiesToRemove.clear();
		}
	}
	
	public void render() {
		terrainRenderer.render(terrain);
		spriteRenderer.render(entities);
		guiRenderer.start();
		gui.render();
		guiRenderer.finish();
	}
	
	public void addEntity(Entity entity) {
		entitiesToAdd.add(entity);
	}
	
	public int getWidth() {
		return width * 16;
	}
	
	public int getHeight() {
		return height * 16;
	}
	
	public Tile getTile(int x, int y) {
		if(x < 0 || x >= getWidth() || y < 0 || y >= getHeight()) {
			return null;
		}
		
		return tiles[x][y];
	}
	
	public PathTileGraph getTileGraph() {
		return tileGraph;
	}
	
	public void setTileGraph(PathTileGraph tileGraph) {
		this.tileGraph = tileGraph;
	}
	
	public void cleanup() {
		terrainRenderer.cleanup();
		spriteRenderer.cleanup();
	}

}
