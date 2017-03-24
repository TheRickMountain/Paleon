package com.paleon.components;

import java.util.List;

import com.paleon.astar.PathAStar;
import com.paleon.core.Game;
import com.paleon.core.JobType;
import com.paleon.core.World;
import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.instances.Bed;
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
	
	private TimeUtil hungerTimer;
	private TimeUtil tirednessTimer;
	
	private Job job;
	
	private Entity resourceInHand;
	
	private static final float MAX_HUNGER = 50;
	private boolean timeToEat = false;
	private float hunger = MAX_HUNGER;
	
	private static final float MAX_TIREDNESS = 100;
	private boolean timeToSleep = false;
	private float tiredness = MAX_TIREDNESS;
	
	private Bed bed = null;
	
	// Hunger per second
	private float hps = 1.0f;
	// Tiredness per second
	private float tps = 0.5f;
	
	public SettlerComponent(Tile tile, Entity parent) {
		super(parent);
		currTile = destTile = nextTile = tile;
		parent.setPosition(tile.getX(), tile.getY());	
		
		firstTimer = new TimeUtil();
		secondTimer = new TimeUtil();
		
		hungerTimer = new TimeUtil();
		tirednessTimer = new TimeUtil();
	}
	
	@Override
	public void update(float dt) {		
		// If settler is hungry, than he chooses food from storage
		if(!timeToEat) {
			calculateHunger();
		}
		
		if(!timeToSleep) {
			calculateTiredness();
		}
		
		hps = 1.0f;
		tps = 1.0f;
		
		System.out.println("Hunger: " + hunger);
		System.out.println("Tiredness: " + tiredness);
		
		if(timeToEat) {
			if(move(dt)) {	
				// Settler is eating for 3 seconds
				double hungerTime = hungerTimer.getTime();
				System.out.println(hungerTime);
				if(hungerTime >= 3) {
					hungerTimer.reset();
					destTile.removeEntityFromWorld();
					hunger = MAX_HUNGER;
					timeToEat = false;
				} else {
					hps = 0.0f;
					tps = 0.25f;
				}
			} else {
				calculateHunger();
				calculateTiredness();
			}
		} else if(timeToSleep) {
			if(move(dt)) {
				// Settler is sleeping for 100 seconds
				if(tirednessTimer.getTime() >= 100) {
					tirednessTimer.reset();
					tiredness = MAX_TIREDNESS;
					timeToSleep = false;
				} else {
					tps = 0.0f;
					hps = 0.25f;
				}
			} else {
				calculateHunger();
				calculateTiredness();
			}
		} else {	
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
	
	private void calculateHunger() {
		if(hungerTimer.getTime() >= 1) {
			hungerTimer.reset();
			
			hunger -= hps;
			
			if(hunger == 0) {
				
				//TODO: Settler is dying
				getParent().remove();
			}
			
			// Settler chooses tile from storage with food
			if(hunger <= 15) {
				for(Tile tile : Game.storageTiles.keySet()) {
					if(Game.storageTiles.get(tile) > 0) {
						if(tile.getEntity().getTag().equals("fish")) {
							pathAStar = new PathAStar(World.getInstance(), currTile, tile);
							if(pathAStar.getLength() > 0) {
								destTile = tile;
								timeToEat = true;
								Game.storageTiles.put(tile, 0);
								return;
							}
						}
					}
				}
			}
		}
	}
	
	private void calculateTiredness() {
		if(tirednessTimer.getTime() >= 1) {
			tirednessTimer.reset();
			
			tiredness -= tps;
			
			if(tiredness == 0) {
				//TODO: Setter is speeping
			}
		
			if(tiredness <= 15) {
				if(bed != null) {
					Tile tile = World.getInstance().getTile((int)bed.getX(), (int)bed.getY());
					pathAStar = new PathAStar(World.getInstance(), currTile, tile);
					if(pathAStar.getLength() > 0) {
						destTile = tile;
						timeToSleep = true;
						return;
					}
				}
			}
		}
	}
	
	@Override
	public ComponentType getType() {
		return ComponentType.SETTLER;
	}

	public Bed getBed() {
		return bed;
	}
	
	public void setBed(Bed bed) {
		this.bed = bed;
	}
	
}
