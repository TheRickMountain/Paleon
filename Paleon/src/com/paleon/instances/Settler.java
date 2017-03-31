package com.paleon.instances;

import java.util.ArrayList;
import java.util.List;

import com.paleon.astar.PathAStar;
import com.paleon.core.Job;
import com.paleon.core.ResourceManager;
import com.paleon.core.World;
import com.paleon.terrain.Tile;
import com.paleon.utils.MathUtils;

public class Settler extends Entity {

	private World world;
	
	private Tile currTile;
	private Tile nextTile;
	private Tile destTile;
	
	private PathAStar pathAStar;
	
	private float movementPerc;	
	private float speed = 2f;
	
	private Job currentJob;
	
	public Settler(World world, Tile tile) {
		super(ResourceManager.getTexture("settler"));
		this.world = world;
		setTag("settler");
		
		currTile = destTile = nextTile = tile;
		setPosition(tile.getX(), tile.getY());
	}

	public void update(float dt) {
		if(currentJob == null) {
			chooseJob();
		} else {
			if(move(dt)) {
				currentJob.getTile().removeEntityFromWorld();
				currentJob = null;
			}
		}
	}
	
	private void chooseJob() {
		for(Job job : world.jobList) {
			Tile tile = job.getTile();
			if(tile.getMovementCost() == 1.0f) {
				pathAStar = new PathAStar(world, currTile, tile);
				if(pathAStar.getLength() != -1) {
					destTile = tile;
					currentJob = job;
					break;
				}
			} else {
				tile = findNearestTile(tile.getNeighbours(false));
				if(tile != null) {
					destTile = tile;
					currentJob = job;
					break;
				}
			}
		}
		
		if(currentJob != null) {
			world.jobList.remove(currentJob);
		}
		
	}
	
	private Tile findNearestTile(List<Tile> tiles) {
		Tile tile = null;
		
		List<Tile> openTiles = new ArrayList<>();
		
		for(Tile t : tiles) {
			if(t.getMovementCost() == 1.0f) {
				openTiles.add(t);
			}
		}
		
		if(openTiles.size() > 0) {
			tile = null;
			float distance = Float.MAX_VALUE;
			
			for(int i = 0; i < openTiles.size(); i++) {
				PathAStar tempPathAStar = new PathAStar(world, currTile, openTiles.get(i));
				float tempDistance = tempPathAStar.getLength();
				
				if(tempDistance != -1 && tempDistance < distance) {
					tile = openTiles.get(i);
					distance = tempDistance;
					pathAStar = tempPathAStar;
				}
			}
		}
		
		return tile;
	}
	
	private boolean move(float dt) {
		if(currTile.equals(destTile)) {
			pathAStar = null;
			return true;
		}
		
		if(nextTile.equals(currTile)) {
			nextTile = pathAStar.getNextTile();
		}
		
		float distToTravel = MathUtils.getDistance(
				currTile.getX(), currTile.getY(), 
				nextTile.getX(), nextTile.getY());
		
		float distThisFrame = speed * dt;
		
		float percThisFrame = distThisFrame / distToTravel;
		
		movementPerc += percThisFrame;
		
		if(movementPerc >= 1) {			
			currTile = nextTile;
			movementPerc = 0;
		}
		
		setPosition(
				MathUtils.getLerp(currTile.getX(), nextTile.getX(), movementPerc), 
				MathUtils.getLerp(currTile.getY(), nextTile.getY(), movementPerc));
		
		return false;
	}
	
}
