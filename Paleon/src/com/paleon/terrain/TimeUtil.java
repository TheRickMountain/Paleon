package com.paleon.terrain;

import org.lwjgl.glfw.GLFW;

public class TimeUtil {

	private double previousTime;
	private boolean ptReceived;
	
	public TimeUtil(){
		reset();
	}
	
	public double getTime(){
		setPreviousTime();
		return GLFW.glfwGetTime() - previousTime;
	}
	
	private void setPreviousTime(){
		if(!ptReceived){
			previousTime = GLFW.glfwGetTime();
			ptReceived = true;
		}
	}
	
	public void reset(){
		previousTime = 0;
		ptReceived = false;
	}
	
}
