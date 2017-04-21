package com.paleon.engine.weather;

import com.paleon.engine.toolbox.Color;

public class Weather {

	private Color fogColor;
	private Color sunLightColor;
	
	private WeatherFrame[] weatherFrames = {
			new WeatherFrame(0, new Color(0.05f, 0.05f, 0.1f), new Color(0f, 0f, 0.26f)),
			new WeatherFrame(2500, new Color(0.05f, 0.05f, 0.1f), new Color(0f, 0f, 0.26f)),
			new WeatherFrame(3500, new Color(0.77f, 0.58f, 0.6f), new Color(1.0f, 0.4f, 0.6f)),
			new WeatherFrame(7500, new Color(0.81f, 0.81f, 0.96f), new Color(1f, 0.9f, 0.8f)),
			new WeatherFrame(12000, new Color(0.91f, 0.91f, 1.0f), new Color(1f, 0.9f, 0.8f)),
			new WeatherFrame(16500, new Color(0.81f, 0.81f, 0.96f), new Color(1f, 0.9f, 0.8f)),
			new WeatherFrame(21000, new Color(0.77f, 0.58f, 0.6f), new Color(1.0f, 0.4f, 0.6f)),
			new WeatherFrame(23500, new Color(0.05f, 0.05f, 0.1f), new Color(0f, 0f, 0.26f)),
			new WeatherFrame(24001, new Color(0.05f, 0.05f, 0.1f), new Color(0f, 0f, 0.26f)),
	};
	
	public void updateWeather(float time) {
		interpolateOtherVariables(time);
	}
	
	private void interpolateOtherVariables(float time) {
		WeatherFrame frame1 = weatherFrames[0];
		WeatherFrame frame2 = null;
		int pointer = 1;
		while (true) {
			frame2 = weatherFrames[pointer++];
			if (time < frame2.getTime()) {
				break;
			} else {
				frame1 = frame2;
			}
		}
		float timeFactor = WeatherFrame.getTimeFactor(frame1, frame2, time);
		updateFogVariables(timeFactor, frame1, frame2);
		updateSunVariables(timeFactor, frame1, frame2);
	}
	
	private void updateFogVariables(float timeFactor, WeatherFrame frame1, WeatherFrame frame2) {
		fogColor = WeatherFrame.getInterpolatedFogColor(frame1, frame2, timeFactor);
	}
	
	private void updateSunVariables(float timeFactor, WeatherFrame frame1, WeatherFrame frame2) {
		sunLightColor = WeatherFrame.getInterpolatedSunLightColour(frame1, frame2, timeFactor);
	}
	
	public Color getFogColor() {
		return fogColor;
	}
	
	public Color getSunLightColor() {
		return sunLightColor;
	}
	
}
