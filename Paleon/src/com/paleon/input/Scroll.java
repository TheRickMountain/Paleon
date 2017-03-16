package com.paleon.input;

import org.lwjgl.glfw.GLFWScrollCallback;

	
public class Scroll extends GLFWScrollCallback {

	private static float scroll;

	public static float getScroll() {
		float scroll = Scroll.scroll;
		Scroll.scroll = 0;
		return scroll;
	}

	@Override
	public void invoke(long window, double scrollX, double scrollY) {
		Scroll.scroll = (float) scrollY;
	}

}

