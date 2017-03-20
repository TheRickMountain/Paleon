package com.paleon.components;

import java.util.Stack;

import com.paleon.astar.PathAStar;
import com.paleon.core.Game;
import com.paleon.core.JobType;
import com.paleon.core.PlantInfo;
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
	
	private Entity resourceInHand;
	
	public SettlerComponent(Tile tile, Entity parent) {
		super(parent);
		currTile = destTile = nextTile = tile;
		parent.setPosition(tile.getX(), tile.getY());	
		
		timer = new TimeUtil();
	}
	
	@Override
	public void update(float dt) {	
		// Settler can't choose the job until his hands aren't empty
		if(resourceInHand != null) {
			if(move(dt)) {
				destTile.addEntityToTile(resourceInHand);
				resourceInHand = null;
			} else {
				resourceInHand.setPosition(getParent().getX(), getParent().getY());
			}
		} else {
			if(job == null) {
				chooseJob();
			} else {
				if(move(dt)) {
					// Character have reached destination tile
					if(timer.getTime() > job.getTime()) {
						if(job.getType().equals(JobType.PRODUCTION)) {
							job.getTarget().removeEntityFromWorld();
							job.getTarget().addEntityToWorld(job.getResultEntity());
							job = null;
						} else if(job.getType().equals(JobType.GATHERING)) {
							resourceInHand = job.getTarget().getEntity();
							job.getTarget().removeEntityFromTile();
							job = null;
							
							Tile tile = chooseStorage();
							if(tile != null) {
								pathAStar = new PathAStar(World.getInstance(), currTile, tile);
								if(pathAStar.getLength() > 0) {
									destTile = tile;
									Game.storageTiles.put(destTile, 1);
								}
							}
						} else if(job.getType().equals(JobType.BUILDING)) {
							job.getTarget().removeEntityFromWorld();
							job.getTarget().addEntityToWorld(job.getResultEntity());
							job = null;
						} else if(job.getType().equals(JobType.GARDEN)) {
							Tile tile = job.getTarget();
							PlantInfo pi = Game.gardenTiles.get(tile);
							if(!pi.isPlowed()) {
								tile.setId(1);
								pi.setPlowed(true);
							} else {
								job = null;
							}
						}
						
						timer.reset();
					}
				}
			}
		}
	}
	
	public void updatePathfinding() {
		if(pathAStar != null) {
			PathAStar tempPathAStar = new PathAStar(World.getInstance(), currTile, destTile);
			if(tempPathAStar.getLength() > 0) {
				pathAStar = tempPathAStar;
			}
		}
	}
	
	private void chooseJob() {
		Stack<Job> jobList = World.getInstance().jobList;
		if(!jobList.empty()) {
			Job tempJob = jobList.peek();
			
			if(tempJob.getType().equals(JobType.PRODUCTION)) {
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
			} else if(tempJob.getType().equals(JobType.GATHERING)) {
				Tile tile = tempJob.getTarget();
				pathAStar = new PathAStar(World.getInstance(), currTile, tile);
				if(pathAStar.getLength() > 0) {
					job = jobList.pop();
					destTile = tile;
				}
			} else if(tempJob.getType().equals(JobType.BUILDING)) {
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
			} else if(tempJob.getType().equals(JobType.GARDEN)) {
				Tile tile = tempJob.getTarget();
				pathAStar = new PathAStar(World.getInstance(), currTile, tile);
				if(pathAStar.getLength() > 0) {
					job = jobList.pop();
					destTile = tile;
				}
			}
		}
	}

	private Tile chooseStorage() {
		for(Tile tile : Game.storageTiles.keySet()) {
			int count = Game.storageTiles.get(tile);
			if(count == 0) {
				return tile;
			}
		}
		
		return null;
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
