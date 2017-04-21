package com.paleon.test;

import java.awt.image.BufferedImage;
import java.util.Random;

public class HeightMapGenerator {

    private static final int OCTAVES = 5;
    private static final float ROUGHNESS = 0.1f;
	private static int size = 256;
	
	private static Random random = new Random();
	private static int seed;
	
	private static BufferedImage I;
	private static int xOffset = 0;
	private static int zOffset = 0;
	
	public static BufferedImage generatePerlin() {
		seed = random.nextInt(1000000000);
		I = new BufferedImage(size, size, BufferedImage.TYPE_INT_RGB);
		
		for(int x = 0; x < size; x++) {
			for(int y = 0; y < size; y++) {
				float color = generatePerlin(x, y);
				color *= 1.45f;
				
				int r = 65536 * (int) (255f * color);
				int g = 256 * (int) (255f * color);
				int b = (int) (255f * color);
				
				int hexColor = r + g + b;
				
				if(hexColor == 0) {
					hexColor = 1;
				}
				
				I.setRGB(x, y, hexColor);
			}
		}
		
		for(int i = 0; i < 10; i++) {
			I = ImageUtils.blur(I);
		}
		
		return I;
	}
	
	public static BufferedImage generatePerlinIsland() {
		seed = random.nextInt(1000000000);
		I = new BufferedImage(size, size, BufferedImage.TYPE_INT_RGB);
		
		for(int x = 0; x < size; x++) {
			for(int y = 0; y < size; y++) {
				float color = generatePerlin(x, y);
				color *= circleIslandMask(x, y);
				color *= 1.45f;
				
				int r = 65536 * (int) (255f * color);
				int g = 256 * (int) (255f * color);
				int b = (int) (255f * color);
				
				int hexColor = r + g + b;
				
				if(hexColor == 0) {
					hexColor = 1;
				}
				
				I.setRGB(x, y, hexColor);
			}
		}
		
		for(int i = 0; i < 3; i++) {
			I = ImageUtils.blur(I);
		}
		
		return I;
	}
	
	public static float circleIslandMask(int x, int z) {
		float distanceX = Math.abs(x - size * 0.5f);
		float distanceY = Math.abs(z - size * 0.5f);
		float distance = (float) Math.sqrt(distanceX * distanceX + distanceY * distanceY);
		
		float max_width = (size * 1.45f) * 0.5f - 10.0f;
		float delta = distance / max_width;
		float gradient = delta * delta;
		
		return Math.max(0.0f, 1.0f - gradient);
	}
	
	public static float squareIslandMask(int x, int z) {
		float distance_x = Math.abs(x - size * 0.5f);
		float distance_y = Math.abs(z - size * 0.5f);
		float distance = Math.max(distance_x, distance_y); // square mask

		float max_width = size * 0.5f - 10.0f;
		float delta = distance / max_width;
		float gradient = delta * delta;

		return Math.max(0.0f, 1.0f - gradient);
	}
	
	private static float generatePerlin(int x, int z) {
		float total = 0;
        float d = (float) Math.pow(2, OCTAVES-1);
        for(int i=0;i<OCTAVES;i++){
            float freq = (float) (Math.pow(2, i) / d);
            float amp = (float) Math.pow(ROUGHNESS, i);
            total += getInterpolatedNoise((x + xOffset) * freq, (z + zOffset) * freq) * amp;
        }
		return (total + 1f) / 2f;
	}
	
	private static float getInterpolatedNoise(float x, float z){
        int intX = (int) x;
        int intZ = (int) z;
        float fracX = x - intX;
        float fracZ = z - intZ;
         
        float v1 = getSmoothNoise(intX, intZ);
        float v2 = getSmoothNoise(intX + 1, intZ);
        float v3 = getSmoothNoise(intX, intZ + 1);
        float v4 = getSmoothNoise(intX + 1, intZ + 1);
        float i1 = interpolate(v1, v2, fracX);
        float i2 = interpolate(v3, v4, fracX);
        return interpolate(i1, i2, fracZ);
    }
	
	private static float interpolate(float a, float b, float blend){
        double theta = blend * Math.PI;
        float f = (float)(1f - Math.cos(theta)) * 0.5f;
        return a * (1f - f) + b * f;
    }
	
	private static float getSmoothNoise(int x, int z) {
        float corners = (getNoise(x - 1, z - 1) + getNoise(x + 1, z - 1) + getNoise(x - 1, z + 1)
                + getNoise(x + 1, z + 1)) / 16f;
        float sides = (getNoise(x - 1, z) + getNoise(x + 1, z) + getNoise(x, z - 1)
                + getNoise(x, z + 1)) / 8f;
        float center = getNoise(x, z) / 4f;
        return corners + sides + center;
    }
	
	private static float getNoise(int x, int z) {
		random.setSeed(x * 49632 + z * 325176 + seed);
		return random.nextFloat() * 2f - 1f;
	}
	
}
