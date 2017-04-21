package com.paleon.test;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

import com.paleon.engine.toolbox.Color;

public class Test {

	private static final float MAX_PIXEL_COLOR = 256 * 256 * 256;
	
	public static void main(String[] args) {
		BufferedImage heightMap = HeightMapGenerator.generatePerlinIsland();
		BufferedImage grassMap = HeightMapGenerator.generatePerlin();
		BufferedImage groundMap = HeightMapGenerator.generatePerlin();
		
		for(int x = 0; x < 256; x++) {
			for(int y = 0; y < 256; y++) {
				float height = groundMap.getRGB(x, y);
		    	height += MAX_PIXEL_COLOR/2f;
		    	height /= MAX_PIXEL_COLOR/2;
				if(height > 0.65f) {
					groundMap.setRGB(x, y, new Color(255, 0, 0).toHex());
				} else {
					groundMap.setRGB(x, y, 0);
				}
			}
		}
		
		for(int x = 0; x < 256; x++) {
			for(int y = 0; y < 256; y++) {
				int color = groundMap.getRGB(x, y);
				if(color == -65536) {
					grassMap.setRGB(x, y, new Color(255, 0, 0).toHex());
				} else {
					grassMap.setRGB(x, y, new Color(0, 255, 0).toHex());
				}
			}
		}
		
		for(int x = 0; x < 256; x++) {
			for(int y = 0; y < 256; y++) {
				float height = heightMap.getRGB(x, y);
		    	height += MAX_PIXEL_COLOR/2f;
		    	height /= MAX_PIXEL_COLOR/2;
				if(height < 0.06f) {
					grassMap.setRGB(x, y, 0);
				}
			}
		}
			
		//grassMap = ImageUtils.blur(grassMap);

		try {
			ImageIO.write(heightMap, "png", new File("resource/biomes/heightmap.png"));
			ImageIO.write(grassMap, "png", new File("resource/biomes/blendmap.png"));
		} catch (IOException e) {

		}
		
	
	}
	
}
