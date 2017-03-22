package com.paleon.core;

import java.util.Map;

import com.paleon.textures.Texture;

public class PlantInfo {

	private int stagesAmount;
	
	private int currentStage;
	private Texture currentTexture;
	
	private Map<Integer, Texture> stages;
	
	private boolean ready;
	private boolean plowed;
	private boolean sowed;
	
	public PlantInfo(Map<Integer, Texture> stages) {
		this.stages = stages;
		
		// First stage texture initialization
		currentTexture = this.stages.get(currentStage);
	}
	
	public int getStagesAmount() {
		return stagesAmount;
	}
	
	public int getStage() {
		return currentStage;
	}
	
	public void addStage(Texture texture, int stage) {
		stages.put(stage, texture);
		stagesAmount = stages.size() - 1;
	}
	
	public void setStage(int stage) {
		if(stage > stagesAmount) {
			stage = stagesAmount;
		} else if(stage < 0) {
			stage = 0;
		}
		
		this.currentStage = stage;
		this.currentTexture = stages.get(stage);
		if(currentStage == stagesAmount) {
			ready = true;
		} else {
			ready = false;
		}
	}
	
	public Texture getTexture() {
		return currentTexture;
	}
	
	public void setPlowed(boolean value) {
		plowed = value;
	}
	
	public boolean isPlowed() {
		return plowed;
	}
	
	public boolean isSowed() {
		return sowed;
	}

	public void setSowed(boolean sowed) {
		this.sowed = sowed;
	}

	public boolean isReady() {
		return ready;
	}
	
}
