package com.paleon.core;

import com.paleon.ecs.Entity;
import com.paleon.graph.Material;
import com.paleon.input.Keyboard;
import com.paleon.input.Mouse;
import com.paleon.terrain.Terrain;

public class MainApp {

	public static void main(String[] args) {
		Display display = new Display("Paleon 0.0.1a", 1152, 648, false);
		
		ResourceLoader.load();
		
		World world = new World();
		world.init();
		
		Entity barrel = new Entity(ResourceManager.getMesh("barrel"),
				new Material(ResourceManager.getTexture("barrel")));
		barrel.getTransform().setScale(0.25f);
		world.addEntity(barrel);
		
		Terrain terrain = new Terrain(0, 0);
		world.addTerrain(terrain);
		
		while(!display.isCloseRequested()) {
			display.pollEvents();
			
			Keyboard.startEventFrame();
			Mouse.startEventFrame();
			
			world.update(display.getDeltaInSeconds());
			
			Keyboard.clearEventFrame();
			Mouse.clearEventFrame();
			
			world.render();
			
			display.swapBuffers();
		}
		
		world.cleanup();
		ResourceManager.cleanup();
		display.shutdown();
	}
	
}
