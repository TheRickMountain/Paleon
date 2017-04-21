package com.paleon.test;

import java.awt.image.BufferedImage;
import java.util.Random;

import com.paleon.engine.toolbox.Color;

public class BlendMapGenerator {

	private static Color[] colors3f;
	
	private static double p = 3;
	private static BufferedImage I;
	private static int px[], py[], color[], cells = 10, size = 256;
	
	public static BufferedImage generate() {
		int n = 0;
		Random rand = new Random();
		I = new BufferedImage(size, size, BufferedImage.TYPE_INT_RGB);
		px = new int[cells];
		py = new int[cells];
		color = new int[cells];
		
		colors3f = new Color[]{
				new Color(255, 0, 0),
				new Color(0, 255, 0),
				new Color(0, 0, 255)
		};
		
		for(int i = 0; i < cells; i++) {
			px[i] = rand.nextInt(size);
			py[i] = rand.nextInt(size);	
			color[i] = colors3f[rand.nextInt(colors3f.length)].toHex();
		}
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				n = 0;
				for (byte i = 0; i < cells; i++) {
					if (distance(px[i], x, py[i], y) < distance(px[n], x, py[n], y)) {
						n = i;
 
					}
				}
				I.setRGB(x, y, color[n]);
			}
		}
		
		return I;
	}
	
	private static double distance(int x1, int x2, int y1, int y2) {
		double d;
	//  d = Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); // Euclidian
	//  d = Math.abs(x1 - x2) + Math.abs(y1 - y2); // Manhattan
		d = Math.pow(Math.pow(Math.abs(x1 - x2), p) + Math.pow(Math.abs(y1 - y2), p), (1 / p)); // Minkovski
	  	return d;
	}
	
}
