package com.paleon.core;

import org.lwjgl.opengl.GL11;

import com.paleon.input.Keyboard;
import com.paleon.input.Mouse;

public class MainApp {
	
	public static void main(String[] args) {
		Display display = new Display("Paleon prototype [0.0.1a]", 1152, 648);
		display.init();
		
		World world = World.getInstance();
		world.init();
		
		while(!display.isCloseRequested()) {
			display.pollEvents();
			
			if(Display.isResized()) {
				display.setResized(false);
			}
			
			// Update
			Keyboard.startEventFrame();
			Mouse.startEventFrame();
			
			world.update(display.getDeltaInSeconds());
			
			Keyboard.clearEventFrame();
			Mouse.clearEventFrame();
			
			// Render
			GL11.glClear(GL11.GL_COLOR_BUFFER_BIT);
			GL11.glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			
			world.render();
			
			display.swapBuffers();
		}
		
		world.cleanup();
		
		display.shutdown();
		
	}

}
