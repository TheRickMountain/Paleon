package com.paleon.components;

import java.util.Stack;

import com.paleon.astar.PathAStar;
import com.paleon.core.World;
import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.terrain.Tile;
import com.paleon.terrain.TimeUtil;
import com.paleon.utils.MathUtils;

public class SettlerComponent extends Component {
	
	private Tile currTile;
	
	private Tile nextTile;
	private Tile destTile;
	
	private PathAStar pathAStar;
	
	private float movementPerc;	
	private float speed = 2f;
	
	private TimeUtil timer;
	private Job job;
	
	public SettlerComponent(Tile tile, Entity parent) {
		super(parent);
		currTile = destTile = nextTile = tile;
		parent.setPosition(tile.getX(), tile.getY());	
		
		timer = new TimeUtil();
	}
	
	@Override
	public void update(float dt) {
		/*if(Mouse.isButtonDown(0)) {
			Tile tile = World.getInstance().getTile(MousePicker.getX(), MousePicker.getY());
			pathAStar = new PathAStar(World.getInstance(), currTile, tile);
			
			if(pathAStar.getLength() > 0) {
				destTile = tile;
			}
		}*/
		
		if(job == null) {
			chooseJob();
		} else {
			// Character have reached destination tile
			if(move(dt)) {
				if(timer.getTime() > job.getTime()) {
					job.getTarget().removeEntity();
					job = null;
					timer.reset();
				}
			}
		}
	}
	
	private void chooseJob() {
		Stack<Job> jobList = World.getInstance().jobList;
		if(!jobList.empty()) {
			Job tempJob = jobList.peek();
			for(Tile tile : tempJob.getTarget().getNeighbours(false)) {
				if(tile != null && !tile.isHasEntity()) {
					pathAStar = new PathAStar(World.getInstance(), currTile, tile);
					if(pathAStar.getLength() > 0) {
						job = jobList.pop();
						destTile = tile;
						break;
					}
				}
			}
		}
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
		
		parent.setPosition(
				MathUtils.getLerp(currTile.getX(), nextTile.getX(), movementPerc), 
				MathUtils.getLerp(currTile.getY(), nextTile.getY(), movementPerc));
		
		return false;
	}

	@Override
	public ComponentType getType() {
		return ComponentType.SETTLER;
	}

}
