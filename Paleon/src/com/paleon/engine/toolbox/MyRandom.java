package com.paleon.engine.toolbox;

import java.util.Random;

public class MyRandom {

	private static final Random rand = new Random();
	
	public static float nextFloat(float range) {
		return rand.nextFloat() * range;
	}
}
