package com.paleon.engine;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.toolbox.MathUtils;


public class Time {

	private static float delta = 0;
	private static long lastFrame = 0;
	private static List<Float> previousTimes = new ArrayList<Float>();
	
	private static final float DELTA_FACTOR = 1000;
	private static final int ROLLING_AVERAGE_LENGTH = 10;
	
	protected static void update() {
		long time = getTime();
		long difference = time - lastFrame;
		float value = ((float) difference) / DELTA_FACTOR;
		delta = updateRollingAverage(value);
		lastFrame = time;
	}
    
    private static float updateRollingAverage(float value) {
		previousTimes.add(0, value);
		if (previousTimes.size() > ROLLING_AVERAGE_LENGTH) {
			previousTimes.remove(ROLLING_AVERAGE_LENGTH);
		}
		if (previousTimes.size() < ROLLING_AVERAGE_LENGTH) {
			return value;
		}
		return MathUtils.getAverageOfList(previousTimes);
	}
    
    private static long getTime() {
        return System.nanoTime() / 1000000;
    }
   
    public static float getDeltaTime() {
    	return delta;
    }
	
}
