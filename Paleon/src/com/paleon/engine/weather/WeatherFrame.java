package com.paleon.engine.weather;

import com.paleon.engine.toolbox.Color;

public class WeatherFrame {

	private float time;
	private Color fogColor;
	private Color sunLightColor;
	
	public WeatherFrame(float time, Color fogColor, Color sunLightColor) {
		this.time = time;
		this.fogColor = fogColor;
		this.sunLightColor = sunLightColor;
	}
	
	protected static float getTimeFactor(WeatherFrame frame1, WeatherFrame frame2, float currentTime){
		float full = frame2.time - frame1.time;
		float progress = currentTime - frame1.time;
		return progress/full;
	}
	
	protected static Color getInterpolatedFogColor(WeatherFrame frame1, WeatherFrame frame2, float timeFactor) {
		return interpolateColor(frame1.fogColor, frame2.fogColor, timeFactor);
	}
	
	protected static Color getInterpolatedSunLightColour(WeatherFrame frame1, WeatherFrame frame2, float timeFactor){
		return interpolateColor(frame1.sunLightColor,frame2.sunLightColor,timeFactor);
	}
	
	private static Color interpolateColor(Color color1, Color color2, float timeFactor){
		float r1 = color1.getR() * (1-timeFactor);
		float g1 = color1.getG() * (1-timeFactor);
		float b1 = color1.getB() * (1-timeFactor);
		float r2 = color2.getR() * timeFactor;
		float g2 = color2.getG() * timeFactor;
		float b2 = color2.getB() * timeFactor;
		return new Color((int) (r1+r2), (int) (g1+g2), (int) (b1+b2));
	}
	
	protected float getTime() {
		return time;
	}
	
}
