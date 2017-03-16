package com.paleon.terrain;

import java.util.ArrayList;
import java.util.List;

import com.paleon.core.World;
import com.paleon.ecs.Entity;

public class Tile {
		
	private Chunk chunk;
	private Terrain terrain;
	
	private int x;
	private int y;
	private int id;
	private Entity entity;
	private boolean hasEntity;
	
	public Tile(Chunk chunk, int x, int y, int id) {
		this.chunk = chunk;
		this.terrain = this.chunk.getTerrain();
		this.x = x;
		this.y = y;
		this.id = id;
	}
	
	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		if(this.id != id) {
			chunk.setRebuild(true);
		}
		this.id = id;
	}
		
	public Entity getEntity() {
		return entity;
	}

	public void addEntity(Entity entity) {
		this.entity = entity;
		this.entity.setPosition(x, y);
		if(this.entity != null) {
			hasEntity = true;
		}
		
		World.getInstance().addEntity(this.entity);
	}
	
	public void removeEntity() {
		this.entity.remove();
		this.entity = null;
		hasEntity = false;
	}
	
	public boolean isHasEntity() {
		return hasEntity;
	}
	
	public float getMovementCost() {
		// 1 - walkable
		// 0 - unwalkable
		
		if(entity != null) {
			return entity.getMovementCost();
		}
		
		return 1;
	}

	public boolean isNeighbour(Tile tile, boolean diag) {
		return Math.abs(this.x - tile.x) + Math.abs(this.y = tile.y) == 1 ||
				(diag && (Math.abs(this.x - tile.x) == 1 && Math.abs(this.y - tile.y) == 1));
	}
	
	public List<Tile> getNeighbours(boolean diags) {
		List<Tile> neighbours = new ArrayList<>();
		
		neighbours.add(terrain.getTile(x, y + 1));
		neighbours.add(terrain.getTile(x + 1, y));
		neighbours.add(terrain.getTile(x, y - 1));
		neighbours.add(terrain.getTile(x - 1, y));
		
		if(diags) {
			neighbours.add(terrain.getTile(x + 1, y + 1));
			neighbours.add(terrain.getTile(x + 1, y - 1));
			neighbours.add(terrain.getTile(x - 1, y - 1));
			neighbours.add(terrain.getTile(x - 1, y + 1));
		}
		
		return neighbours;
	}
	
}
