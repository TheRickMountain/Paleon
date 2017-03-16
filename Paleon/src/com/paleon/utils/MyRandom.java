package com.paleon.utils;

import java.util.Random;

public class MyRandom {
	
	private static Random rand = new Random();
	
	public static float nextFloat(float min, float max) {
		return min + (max - min) * rand.nextFloat();
	}
	
	public static float nextFloat() {
		return rand.nextFloat();
	}
	
	public static float nextFloat(float range) {
		return rand.nextFloat() * range;
	}
	
	public static int nextInt(int min, int max) {
		return (nextInt(max - min) + min) + 1;
	}
	
	public static int nextInt(int[] numbers) {
		return numbers[nextInt(numbers.length)];
	}
	
	public static int nextInt(int range) {
		return rand.nextInt(range);
	}

}
