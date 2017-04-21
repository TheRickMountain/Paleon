package com.paleon.engine;

public class Main {
	
	public static void main(String[] args) throws Exception {
		try{
			PaleonEngine game = new PaleonEngine();
			game.start();
		} catch(Exception e){
			e.printStackTrace();
			System.exit(-1);
		}
	}
}
