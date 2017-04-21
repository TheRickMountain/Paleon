package com.paleon.engine.input;

import static org.lwjgl.glfw.GLFW.*;

import java.util.ArrayList;
import java.util.List;

import org.lwjgl.glfw.GLFWKeyCallback;

public class Keyboard extends GLFWKeyCallback{
	
	private static List<Integer> events          = new ArrayList<>();
    private static List<Integer> eventsThisFrame = new ArrayList<>();
    private static List<Integer> eventsLastFrame = new ArrayList<>();
    
    public static boolean isKeyDown(int key) {
        return eventsThisFrame.contains(key) && !eventsLastFrame.contains(key);
    }
    
    public static boolean isKeyUp(int key) {
    	return !eventsThisFrame.contains(key) && eventsLastFrame.contains(key);
    }
    
    public static boolean isKey(int key) {
        return eventsThisFrame.contains(key);
    }
    
    public static void startEventFrame() {
        eventsThisFrame.clear();
        eventsThisFrame.addAll(events);
    }

    public static void clearEventFrame() {
        eventsLastFrame.clear();
        eventsLastFrame.addAll(eventsThisFrame);
    }
	
	@Override
	public void invoke(long window, int key, int scancode, int action, int mods) {
		Keyboard.setKey(key, action != GLFW_RELEASE);
		
		if ( key == GLFW_KEY_ESCAPE && action == GLFW_RELEASE )
			glfwSetWindowShouldClose(window, true);

        for (int mod : Key.MODIFIERS)
            if ((mods & mod) == mod)
                Keyboard.setKey(mod, action != GLFW_RELEASE);
	}
	
	 public static void setKey(int key, boolean pressed) {
	        if (pressed && !events.contains(key))
	            events.add(key);

	        if (!pressed && events.contains(key))
	            events.remove((Integer) key);
	 }

}