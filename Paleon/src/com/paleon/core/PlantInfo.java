package com.paleon.core;

public class PlantInfo {

	private int stage;
	private boolean ready;
	private boolean plowed;
	
	public PlantInfo() {
	}
	
	public void setStage(int stage) {
		this.stage = stage;
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
