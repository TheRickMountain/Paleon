package com.paleon.engine.terrain;

import java.awt.image.BufferedImage;
import java.io.IOException;

import javax.imageio.ImageIO;

import com.paleon.maths.vecmath.Vector3f;

public class TerrainLoader {
	
	private static final float MAX_HEIGHT = 40;
	private static float [][] heights;
	private static final float MAX_PIXEL_COLOR = 256 * 256 * 256;
	
	protected static TerrainVertex[][] load(String heightmap) {
		BufferedImage image = null;
		try {
			image = ImageIO.read(TerrainLoader.class.getClass().getResourceAsStream(heightmap));
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		int vertexCount = image.getHeight() + 1;
    	heights = new float[vertexCount][vertexCount];
		TerrainVertex[][] terrainVertices = new TerrainVertex[vertexCount][vertexCount];
		for(int i=0;i<vertexCount;i++){
			for(int j=0;j<vertexCount;j++){
				float height = getHeight(j, i, image);
				heights[i][j] = height;
				terrainVertices[i][j] = new TerrainVertex(height, calculateNormal(j, i, image));
			}
		}
		return terrainVertices;
	}
    
    private static Vector3f calculateNormal(int x, int z, BufferedImage image){
    	float heightL = getHeight(x - 1, z, image);
    	float heightR = getHeight(x + 1, z, image);
    	float heightD = getHeight(x, z - 1, image);
    	float heightU = getHeight(x, z + 1, image);
    	Vector3f normal = new Vector3f(heightL - heightR, 2f, heightD - heightU);
    	normal.normalise();
    	return normal;
    }
    
    private static float getHeight(int x, int y, BufferedImage image) {
    	if(x < 0 || x >= image.getHeight() || y < 0 || y >= image.getHeight()) {
    		return 0;
    	}
    	float height = image.getRGB(x, y);
    	height += MAX_PIXEL_COLOR/2f;
    	height /= MAX_PIXEL_COLOR/2f;
    	height *= MAX_HEIGHT;
    	return height;
    }
    
    public float[][] getHeights(){
    	return heights;
    }
    
}
