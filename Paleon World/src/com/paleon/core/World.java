package com.paleon.core;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.paleon.ecs.Entity;
import com.paleon.graph.Mesh;
import com.paleon.math.Vector3f;
import com.paleon.renderer.RenderEngine;
import com.paleon.terrain.Terrain;
import com.paleon.terrain.TerrainBlock;
import com.paleon.toolbox.OpenglUtils;

public class World {
	
	private Camera camera;
	
	private RenderEngine renderer;
	
	private List<Entity> entities = new ArrayList<Entity>();
	private List<Entity> entitiesToRemove = new ArrayList<Entity>();
	private List<Entity> entitiesToAdd = new ArrayList<Entity>();
	
	private Map<Mesh, List<Entity>> entitiesToRender = new HashMap<Mesh, List<Entity>>();
	
	public Map<Terrain, List<TerrainBlock>> terrains = new HashMap<Terrain, List<TerrainBlock>>();
	
	private static final int WORLD_TERRAIN_WIDTH = 3;
	private static final int WORLD_TILE_WIDTH = WORLD_TERRAIN_WIDTH * Terrain.TERRAIN_WIDTH_BLOCKS;
	
	private final TerrainBlock[][] terrainGrid;
	
	public World() {
		camera = new Camera(new Vector3f(0, 0.25f, 0));
		renderer = new RenderEngine(camera);
		
		terrainGrid = new TerrainBlock[WORLD_TILE_WIDTH][WORLD_TILE_WIDTH];
		
		OpenglUtils.depthTest(true);
		OpenglUtils.cullBackFaces(true);
	}
	
	public void init() {
		
	}
	
	public void update(float dt) {
		camera.update(dt);
		
		for(Entity entity : entities) {
			entity.update(dt);
		}
		
		if(!entitiesToAdd.isEmpty()) {
			entities.addAll(entitiesToAdd);
			entitiesToAdd.clear();
		}
		
		if(!entitiesToRemove.isEmpty()) {
			entities.removeAll(entitiesToRemove);
			entitiesToRemove.clear();
		}
	}
	
	public void render() {
		renderer.render(entitiesToRender, terrains);
	}
	
	public void addEntity(Entity entity) {
		entity.init();
		
		this.entitiesToAdd.add(entity);
		
		if(entity.getMesh() != null) {
			List<Entity> batch = entitiesToRender.get(entity.getMesh());
			if(batch == null) {
				batch = new ArrayList<Entity>();
	            entitiesToRender.put(entity.getMesh(), batch);
			} 
			batch.add(entity);
		}
	}
	
	public void removeEntity(Entity entity) {
		this.entitiesToRemove.add(entity);
		
		List<Entity> batch = entitiesToRender.get(entity.getMesh());
		batch.remove(entity);
		
		if(batch.isEmpty()) {
			entitiesToRender.remove(entity.getMesh());
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
	
	public void cleanup() {
		renderer.cleanup();
	}

}
