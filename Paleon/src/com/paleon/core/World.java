package com.paleon.core;

import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

import org.lwjgl.opengl.GL11;

import com.paleon.astar.PathTileGraph;
import com.paleon.components.Job;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.renderer.SpriteRenderer;
import com.paleon.renderer.TerrainRenderer;
import com.paleon.terrain.Terrain;
import com.paleon.terrain.Tile;
import com.paleon.utils.MousePicker;

public class World {
	
	private static final World INSTANCE = new World();
	
	private Camera camera;
	
	private TerrainRenderer terrainRenderer;
	private SpriteRenderer spriteRenderer;
	
	private List<Entity> entities = new ArrayList<Entity>();
	private List<Entity> entitiesToRemove = new ArrayList<Entity>();
	private List<Entity> entitiesToAdd = new ArrayList<Entity>();
	
	private int width = 2;
	private int height = 2;
	
	private Terrain terrain;
	
	private Tile[][] tiles;
	
	public Stack<Job> jobList = new Stack<>();
	public List<Entity> settlersList = new ArrayList<>();
	
	private Game game;
	
	private PathTileGraph tileGraph;
	
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
		
		terrain = new Terrain(width, height);
		tiles = new Tile[getWidth()][getHeight()];
		for(int i = 0; i < getWidth(); i++) {
			for(int j = 0; j < getHeight(); j++) {
				tiles[i][j] = terrain.getTile(i, j);
			}
		}
		
		game = new Game();
	}
	
	public void update(float dt) {
		MousePicker.update();
		camera.update(dt);
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
	}
	
	public void addEntity(Entity entity) {
		if(entity.hasComponent(ComponentType.SETTLER)) {
			settlersList.add(entity);
		}
		
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
