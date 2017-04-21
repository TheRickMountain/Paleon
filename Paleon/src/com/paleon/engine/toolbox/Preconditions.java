package com.paleon.engine.toolbox;

public class Preconditions {

	public static void checkArgument(boolean expression) {
	    if (!expression) {
	      throw new IllegalArgumentException();
	    }
	}
	
	public static void checkArgument(boolean expression, Object errorMessage) {
	    if (!expression) {
	      throw new IllegalArgumentException(String.valueOf(errorMessage));
	    }
	}
	
}
