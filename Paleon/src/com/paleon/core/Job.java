package com.paleon.core;

import com.paleon.terrain.Tile;

public class Job {
	
	private Tile tile;
	private JobType type;
	
	public Job(Tile tile, JobType type) {
		this.tile = tile;
		this.type = type;
	}
	
	public Tile getTile() {
		return tile;
	}
	
	public JobType getType() {
		return type;
	}

}
