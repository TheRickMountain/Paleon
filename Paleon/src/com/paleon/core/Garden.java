package com.paleon.core;

import java.util.ArrayList;
import java.util.List;

import com.paleon.components.Job;
import com.paleon.ecs.Entity;
import com.paleon.terrain.Tile;
import com.paleon.terrain.TimeUtil;

public class Garden {
	
	private List<Tile> tiles = new ArrayList<>();
	private TimeUtil timer;
	
	private boolean plowingInJobList = false;
	private boolean sowingInJobList = false;
	private boolean harvestingInJobList = false;
	
	private int stage = 0;
	
	public Garden(List<Tile> tiles) {
		this.tiles.addAll(tiles);
		timer = new TimeUtil();
	}
	
	public void update() {
		if(!plowingInJobList) {
			for(Tile tile : tiles) {
				World.getInstance().jobList.add(new Job(tile, 0.5f, JobType.PLOWING, null));
			}
			plowingInJobList = true;
		}
		
		if(!sowingInJobList) {
			if(isPlowed()) {
				for(Tile tile : tiles) {
					World.getInstance().jobList.add(new Job(tile, 0.5f, JobType.SEEDING, new Entity(Game.wheat.get(stage))));
				}
				sowingInJobList = true;
			}
		}
		
		if(!harvestingInJobList) {
			if(isSowed()) {
				if(timer.getTime() > 5) {
					stage++;
					
					for(Tile tile : tiles) {
						tile.getEntity().setTexture(Game.wheat.get(stage));
					}
					
					if(stage == 7) {
						harvestingInJobList = true;
						for(Tile tile : tiles) {
							World.getInstance().jobList.add(new Job(tile, 0.0f, JobType.GATHERING, null));
						}
					}
					
					timer.reset();
				}
			}
		}
		
		if(harvestingInJobList) {
			if(isHarvested()) {
				sowingInJobList = false;
				harvestingInJobList = false;
				stage = 0;
				
			}
		}
	}
	
	public boolean isPlowed() {
		for(Tile tile : tiles) {
			if(tile.getId() != 1) return false;
		}
		return true;
	}
	
	public boolean isSowed() {
		for(Tile tile : tiles) {
			if(!tile.isHasEntity()) return false;
		}
		
		return true;
	}
	
	public boolean isHarvested() {
		for(Tile tile : tiles) {
			if(tile.isHasEntity()) return false;
		}
		
		return true;
	}

}
