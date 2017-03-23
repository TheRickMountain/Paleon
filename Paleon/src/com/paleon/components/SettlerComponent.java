package com.paleon.components;

import java.util.List;

import com.paleon.astar.PathAStar;
import com.paleon.core.Game;
import com.paleon.core.JobType;
import com.paleon.core.World;
import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.instances.Fish;
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
	
	private TimeUtil firstTimer;
	private TimeUtil secondTimer;
	private Job job;
	
	private Entity resourceInHand;
	
	public SettlerComponent(Tile tile, Entity parent) {
		super(parent);
		currTile = destTile = nextTile = tile;
		parent.setPosition(tile.getX(), tile.getY());	
		
		firstTimer = new TimeUtil();
		secondTimer = new TimeUtil();
	}
	
	@Override
	public void update(float dt) {		
		// Settler can't choose the job until his hands aren't empty
		if(resourceInHand != null) {
			if(move(dt)) {
				destTile.addEntityToTile(resourceInHand);
				resourceInHand = null;
				
				if(job != null) {
					if(job.getType().equals(JobType.FISHING)) {
						Tile tile = getPath(job.getTarget().getNeighbours(false));
						if(tile != null) {
							destTile = tile;
						}
					}
				}
				
			} else {
				resourceInHand.setPosition(getParent().getX(), getParent().getY());
			}
		} else {
			if(job == null) {
				chooseJob();
			} else {
				if(move(dt)) {
					// Character have reached destination tile
					if(firstTimer.getTime() > job.getTime()) {
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
						} else if(job.getType().equals(JobType.PLOWING)) {
							Tile tile = job.getTarget();
							tile.setId(1);
							job = null;
						} else if(job.getType().equals(JobType.SEEDING)) {
							job.getTarget().addEntityToWorld(job.getResultEntity());
							job = null;
						} else if(job.getType().equals(JobType.FISHING)) {	
							if(secondTimer.getTime() > 5) {
								secondTimer.reset();
								
								resourceInHand = new Fish();
								World.getInstance().addEntity(resourceInHand);
								
								Tile tile = chooseStorage();
								if(tile != null) {
									pathAStar = new PathAStar(World.getInstance(), currTile, tile);
									if(pathAStar.getLength() > 0) {
										destTile = tile;
										Game.storageTiles.put(destTile, 1);
									}
								}
							}
						}
						
						firstTimer.reset();
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
		List<Job> jobList = World.getInstance().jobList;
		if(!jobList.isEmpty()) {
			Job tempJob = jobList.get(0);
			
			// If tile is unwalkable, then choose one available tile from neighbours
			if(tempJob.getTarget().getMovementCost() == 0.0f) {
				Tile tile = getPath(tempJob.getTarget().getNeighbours(false));
				if(tile != null) {
					job = jobList.remove(0);
					destTile = tile;
				}
			} else {
				Tile tile = tempJob.getTarget();
				pathAStar = new PathAStar(World.getInstance(), currTile, tile);
				if(pathAStar.getLength() > 0) {
					job = jobList.remove(0);
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

	private Tile getPath(List<Tile> neighbours) {
		for(Tile tile : neighbours) {
			if(tile != null) {
				boolean walkable = false;
				if(tile.isHasEntity()) {
					if(tile.getEntity().isWalkable()) {
						walkable = true;
					}
				} else {
					walkable = true;
				}
				
				if(tile.getId() == 5) {
					walkable = true;
				}
				
				if(walkable) {
					pathAStar = new PathAStar(World.getInstance(), currTile, tile);
					if(pathAStar.getLength() > 0) {
						return tile;
					}
				}
			}
		}
		
		return null;
	}
	
	@Override
	public ComponentType getType() {
		return ComponentType.SETTLER;
	}

}
