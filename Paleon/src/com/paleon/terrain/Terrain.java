package com.paleon.terrain;

import java.util.ArrayList;
import java.util.List;

import com.paleon.textures.Texture;
import com.paleon.utils.MyFile;

public class Terrain {
	
	private List<Chunk> chunks = new ArrayList<Chunk>();
	
	private Texture atlas;
	
	private int width;
	private int height;
	
	public Terrain(int width, int height) {
		this.width = width * 16;
		this.height = height * 16;
		
		atlas = Texture.newTexture(new MyFile("sprites/atlas.png"))
				.nearestFiltering().normalMipMap().clampEdges().build();
		
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				chunks.add(new Chunk(this, x, y));
			}
		}

	}
	
	public Tile getTile(int x, int y) {
		int terrX = x / 16;
		int terrY = y / 16;
		
		int tileX = x - terrX * 16;
		int tileY = y - terrY * 16;
		
		if(x < 0 || x >= width || y < 0 || y >= height)
			return null;
		
		return chunks.get(terrX * (width / 16) + terrY).getTile(tileX, tileY);
	}
	
	public List<Chunk> getChunks() {
		return chunks;
	}
	
	public Texture getAtlas() {
		return atlas;
	}
	
	public int getWidth() {
		return width;
	}

	public int getHeight() {
		return height;
	}

	public void cleanup() {
		for(Chunk chunk : chunks) {
			chunk.cleanup();
		}
	}

}
