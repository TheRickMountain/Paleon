package com.paleon.engine.toolbox;

public class GameTime {

	private static TimeUtil timer;
	
	private static int hour = 0;
	private static int minute = 0;
	
	private static int tempMinute = 0;
	
	public static void init(){
		timer = new TimeUtil();
	}
	
	public static void update(){
		minute = (int)timer.getTime() + tempMinute;
		if(minute == 60) {
			minute = 0;
			tempMinute = 0;
			timer.reset();
			hour++;
		}
		if(hour == 24) {
			hour = 0;
		}
	}

	public static int getMinute() {
		return minute;
	}
	
	public static int getHour() {
		return hour;
	}
	
	public static void setTime(int h, int m){
		if(h >= 24) {
			h = 0;
		} else if(h < 0) {
			h = 0;
		}
		
		if(m >= 60) {
			m = 0;
		} else if(m < 0) {
			m = 0;
		}
		hour = h;
		tempMinute = m;
	}
	
	public static String getTime(){
		String hourStr = String.valueOf(hour);
		if(hourStr.length() == 1) {
			hourStr = "0" + hourStr;
		}
		
		String minuteStr = String.valueOf(minute);
		if(minuteStr.length() == 1) {
			minuteStr = "0" + minuteStr;
		}
		
		return hourStr + ":" + minuteStr;
	}
	
	public static int getATime() {
		return hour * 1000 + (int)((minute * 10) * 1.666666666f);
	}
	
}
