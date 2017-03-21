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
	private Entity prototype;
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

	public void addEntityToWorld(Entity entity) {
		addEntityToTile(entity);
		
		World.getInstance().addEntity(entity);
	}
	
	public void addEntityToTile(Entity entity) {
		this.entity = entity;
		this.entity.setPosition(x, y);
		hasEntity = true;
	}
	
	public void removeEntityFromWorld() {
		this.entity.remove();
		removeEntityFromTile();
	}
	
	public void removeEntityFromTile() {
		this.entity = null;
		hasEntity = false;
	}
	
	public Entity getPrototype() {
		return prototype;
	}
	
	public void addPrototype(Entity prototype) {
		this.prototype = prototype;
		this.prototype.setPosition(x, y);
		
		World.getInstance().addEntity(this.prototype);
	}
	
	public void removePrototype() {
		this.prototype.remove();
		this.prototype = null;
	}
	
	public boolean isHasEntity() {
		return hasEntity;
	}
	
	public float getMovementCost() {
		// 1 - walkable
		// 0 - unwalkable
		
		if(entity != null) {
			return entity.isWalkable() ? 1 : 0;
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
