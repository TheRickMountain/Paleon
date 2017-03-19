package com.paleon.components;

import com.paleon.core.JobType;
import com.paleon.ecs.Entity;
import com.paleon.terrain.Tile;

public class Job {
	
	private Tile target;
	private float time;
	private JobType type;
	private Entity resultEntity;
	
	public Job(Tile target, float time, JobType type, Entity resultEntity) {
		this.target = target;
		this.time = time;
		this.type = type;
		this.resultEntity = resultEntity;
	}

	public Tile getTarget() {
		return target;
	}

	public float getTime() {
		return time;
	}
	
	public JobType getType() {
		return type;
	}
	
	public Entity getResultEntity() {
		return resultEntity;
	}
	
}
