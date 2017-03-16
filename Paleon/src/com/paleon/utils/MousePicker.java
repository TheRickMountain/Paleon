package com.paleon.utils;

import com.paleon.core.Camera;
import com.paleon.core.Display;
import com.paleon.input.Mouse;

public class MousePicker {
	
	private static float x;
	private static float y;
	
	private static Camera camera;
	
	private MousePicker() {}
	
	public static void init(Camera cam) {
		camera = cam;
	}
	
	public static void update() {
		x = (Mouse.getX() * camera.getZoom()) + (camera.getX() - ((Display.getWidth() * camera.getZoom()) / 2));
		y = (Mouse.getY() * camera.getZoom()) + (camera.getY() - ((Display.getHeight() * camera.getZoom()) / 2));
	}
	
	public static int getX() {
		return (int) x;
	}
	
	public static int getY() {
		return (int) y;
	}
	
	public static float getXf() {
		return x;
	}
	
	public static float getYf() {
		return y;
	}
	
}
