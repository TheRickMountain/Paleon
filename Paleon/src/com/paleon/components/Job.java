package com.paleon.components;

import com.paleon.blueprints.Blueprint;
import com.paleon.ecs.Entity;
import com.paleon.terrain.Tile;

public class Job {

	public static enum JobType {
		BUILDING, 
		GATHERING,
		MINING,
		CHOPPING,
		STORAGE_SELECTION
	}
	
	private Tile tile;
	private JobType type;
	private float time;
	private Blueprint blueprint;
	
	public Job(Tile tile, JobType type, float time, Blueprint blueprint) {
		this.tile = tile;
		this.type = type;
		this.time = time;
		this.blueprint = blueprint;
	}

	public Tile getTile() {
		return tile;
	}

	public JobType getType() {
		return type;
	}

	public float getTime() {
		return time;
	}
	
	public Entity getEntity() {
		return blueprint.getInstance();
	}
	
}
