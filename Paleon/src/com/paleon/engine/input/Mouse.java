package com.paleon.engine.input;

import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_1;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_2;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_3;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_4;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_5;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_6;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_7;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_8;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_LAST;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_LEFT;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_MIDDLE;
import static org.lwjgl.glfw.GLFW.GLFW_MOUSE_BUTTON_RIGHT;
import static org.lwjgl.glfw.GLFW.GLFW_RELEASE;

import java.util.ArrayList;
import java.util.List;

import org.lwjgl.glfw.GLFW;
import org.lwjgl.glfw.GLFWCursorPosCallback;
import org.lwjgl.glfw.GLFWMouseButtonCallback;
import org.lwjgl.glfw.GLFWScrollCallback;

import com.paleon.engine.Display;
import com.paleon.maths.vecmath.Vector2f;

public class Mouse extends GLFWMouseButtonCallback {
	
	private static boolean grabbed = false;
	
	public static class Scroll extends GLFWScrollCallback {

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
	
	public static class Cursor extends GLFWCursorPosCallback {

		private static float x, y, dx, dy;
		
	    public static float getX() {
	        return x;
	    }

	    public static float getY() {
	        return y;
	    }
	    
	    public static float getDX() {
	        float dx = Cursor.dx;
	        Cursor.dx = 0;
	        return dx;
	    }
	   
	    public static float getDY() {
	        float dy = Cursor.dy;
	        Cursor.dy = 0;
	        return dy;
	    }
	    
	    public static Vector2f getNDC() {
	    	float x = (2.0f * getX()) / Display.getWidth() - 1.0f;
			float y = 1.0f - (2.0f * getY()) / Display.getHeight();
			return new Vector2f(x, y);
	    }
	    
		@Override
		public void invoke(long window, double x, double y) {
			Cursor.dx = (float) x - Cursor.x;
			Cursor.dy = (float) y - Cursor.y;

			Cursor.x = (float) x;
			Cursor.y = (float) y;
		}
		
	}
	
    public static final int BUTTON_1 = GLFW_MOUSE_BUTTON_1;
    public static final int BUTTON_2 = GLFW_MOUSE_BUTTON_2;
    public static final int BUTTON_3 = GLFW_MOUSE_BUTTON_3;
    public static final int BUTTON_4 = GLFW_MOUSE_BUTTON_4;
    public static final int BUTTON_5 = GLFW_MOUSE_BUTTON_5;
    public static final int BUTTON_6 = GLFW_MOUSE_BUTTON_6;
    public static final int BUTTON_7 = GLFW_MOUSE_BUTTON_7;
    public static final int BUTTON_8 = GLFW_MOUSE_BUTTON_8;

    public static final int LEFT_BUTTON   = GLFW_MOUSE_BUTTON_LEFT;
    public static final int RIGHT_BUTTON  = GLFW_MOUSE_BUTTON_RIGHT;
    public static final int MIDDLE_BUTTON = GLFW_MOUSE_BUTTON_MIDDLE;

    public static final int BUTTON_LAST = GLFW_MOUSE_BUTTON_LAST;
	
	private static List<Integer> events          = new ArrayList<>();
    private static List<Integer> eventsThisFrame = new ArrayList<>();
    private static List<Integer> eventsLastFrame = new ArrayList<>();
	
    public static boolean isButtonDown(int button) {
        return eventsThisFrame.contains(button) && !eventsLastFrame.contains(button);
    }
    
    public static boolean isButtonUp(int button) {
        return !eventsThisFrame.contains(button) && eventsLastFrame.contains(button);
    }
    
    public static boolean isButton(int button) {
        return eventsThisFrame.contains(button);
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
	public void invoke(long window, int button, int action, int mods) {
		Mouse.setButton(button, action != GLFW_RELEASE);

        for (int mod : Key.MODIFIERS)
            if ((mods & mod) == mod)
                Keyboard.setKey(mod, true);
	}
	
	 public static void setButton(int button, boolean pressed) {
		 if (pressed && !events.contains(button)){
			 events.add(button);
		 }

		 if (!pressed && events.contains(button)){
			 events.remove((Integer) button);
		 }
	 }
	 
	 public static float getScroll(){
		 return Scroll.getScroll();
	 }
	 
	 public static float getX(){
		 return Cursor.getX();
	 }

	 public static float getY(){
		 return Cursor.getY();
	 }
	 
	 public static float getDX(){
		 return Cursor.getDX();
	 }
	 
	 public static float getDY(){
		 return Cursor.getDY();
	 }
	 
	 public static Vector2f getNDC(){
		 return Cursor.getNDC();
	 }
	 
	 public boolean isGrabbed() {
		 return grabbed;
	 }
	 
	 public static void show() {
		 GLFW.glfwSetInputMode(Display.getWindow(), GLFW.GLFW_CURSOR, GLFW.GLFW_CURSOR_NORMAL);
	 }
	 
	 public static void hide() {
		 GLFW.glfwSetInputMode(Display.getWindow(), GLFW.GLFW_CURSOR, GLFW.GLFW_CURSOR_DISABLED);
	 }
	 
}