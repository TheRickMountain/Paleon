package com.paleon.core;

import com.paleon.input.Mouse;
import com.paleon.renderer.GUIRenderer;
import com.paleon.utils.Rect;

public class GUI {
	
	private GUIRenderer renderer;
	
	private GUITexture gardening;
	private GUITexture storage;
	private GUITexture gathering;
	private GUITexture chopping;
	private GUITexture mining;
	private GUITexture building;
	private GUITexture fishing;
	
	public GUI(GUIRenderer renderer) {
		this.renderer = renderer;
		
		gardening = new GUITexture(ResourceManager.getTexture("gardening"), new Rect(0, 0, 64, 64));
		storage = new GUITexture(ResourceManager.getTexture("storage"), 	new Rect(64, 0, 64, 64));
		gathering = new GUITexture(ResourceManager.getTexture("gathering"), new Rect(128, 0, 64, 64));
		chopping = new GUITexture(ResourceManager.getTexture("chopping"), 	new Rect(192, 0, 64, 64));
		mining = new GUITexture(ResourceManager.getTexture("mining"), 		new Rect(256, 0, 64, 64));
		building = new GUITexture(ResourceManager.getTexture("building"), 	new Rect(320, 0, 64, 64));
		fishing = new GUITexture(ResourceManager.getTexture("fishing"), 	new Rect(384, 0, 64, 64));

	}
	
	public void update(float dt) {
		Mouse.setActiveInGUI(false);
		
		/*if(Mouse.isButtonDown(0)) {
			if(gardening.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.PLOWING;
			} else if(storage.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.STORAGE;
			} else if(gathering.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.GATHERING;
			} else if(chopping.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.PRODUCTION;
			} else if(mining.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.PRODUCTION;
			} else if(building.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.BUILDING;
			} else if(fishing.rect.isMouseOvered()) {
				Mouse.setActiveInGUI(true);
				Game.jobType = JobType.FISHING;
			}
			
			System.out.println(Game.jobType.toString());
		}*/
	}
	
	public void render() {
		renderer.render(gardening);
		renderer.render(storage);
		renderer.render(gathering);
		renderer.render(chopping);
		renderer.render(mining);
		renderer.render(building);
		renderer.render(fishing);
	}

}
