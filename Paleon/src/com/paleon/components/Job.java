package com.paleon.components;

import com.paleon.terrain.Tile;

public class Job {
	
	private Tile target;
	private float time;
	
	public Job(Tile target, float time) {
		this.target = target;
		this.time = time;
	}

	public Tile getTarget() {
		return target;
	}

	public float getTime() {
		return time;
	}
	
}
