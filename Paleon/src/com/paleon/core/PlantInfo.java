package com.paleon.core;

import java.util.HashMap;
import java.util.Map;

import com.paleon.textures.Texture;

public class PlantInfo {

	private int stagesAmount;
	
	private int currentStage;
	private Texture currentTexture;
	
	private Map<Integer, Texture> stages = new HashMap<>();
	
	private boolean ready;
	private boolean plowed;
	
	public PlantInfo() {
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
		return ResourceManager.getTexture("wheat_stage_0");
	}
	
	public void setPlowed(boolean value) {
		plowed = value;
	}
	
	public boolean isPlowed() {
		return plowed;
	}
	
	public boolean isReady() {
		return ready;
	}
	
}
