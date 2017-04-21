package com.paleon.engine.toolbox;

import org.lwjgl.glfw.GLFW;

public class TimeUtil {

	private double previousTime;
	private boolean ptReceived;
	
	public TimeUtil(){
		reset();
	}
	
	public double getTime(){
		setPreviouseTime();
		return GLFW.glfwGetTime() - previousTime;
	}
	
	private void setPreviouseTime(){
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
