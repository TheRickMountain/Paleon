package com.paleon.terrain;

import java.nio.FloatBuffer;
import java.util.ArrayList;
import java.util.List;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;
import org.lwjgl.system.MemoryUtil;

import com.paleon.utils.MathUtils;

public class Chunk {

	private Terrain terrain;
	
	public static final int CHUNK_SIZE = 16;
	public static final float SPRITE_SHEET = 4;	
	
	private int x;
	private int y;
	
	private Tile[][] tiles;
	
	private int vao;
	private int vVbo;
	private int tcVbo;
	
	private float[] vArr;
	private float[] tcArr;
	
	private boolean rebuild = false;
	
	public Chunk(Terrain terrain, int x, int y) {
		this.terrain = terrain;
		
		this.x = x * CHUNK_SIZE;
		this.y = y * CHUNK_SIZE;
		
		tiles = new Tile[CHUNK_SIZE][CHUNK_SIZE];
		for(int i = 0; i < CHUNK_SIZE; i++) {
			for(int j = 0; j < CHUNK_SIZE; j++) {
				tiles[i][j] = new Tile(this, i + this.x, j + this.y, 0);
			}
		}
		
		create();
		initGL();
	}
	
	private void create() {
		List<Float> vertices = new ArrayList<>();
		List<Float> textureCoords = new ArrayList<>();
		
		float startx = 0.0f;
		float starty = 0.0f;
		for(int i = 0; i < CHUNK_SIZE * CHUNK_SIZE; i++) {
			float u0 = ((tiles[(int) startx][(int) starty].getId() % (int)SPRITE_SHEET) / SPRITE_SHEET);
	        float u1 = (u0 + (1.0f / SPRITE_SHEET));
	        float v0 = ((tiles[(int) startx][(int) starty].getId() / (int)SPRITE_SHEET) / SPRITE_SHEET);
	        float v1 = (v0 + (1.0f / SPRITE_SHEET));
			
			// Left Top vertex
			vertices.add(startx + x); // x
	        vertices.add(starty + y); // y
	        textureCoords.add(u0);
	        textureCoords.add(v0);
	        
	        // Left Bottom vertex
	        vertices.add(startx + x); // x
	        vertices.add(starty + 1.0f + y); // y
	        textureCoords.add(u0);
	        textureCoords.add(v1);
	        
	        // Right Bottom vertex
	        vertices.add(startx + 1.0f + x); // x
	        vertices.add(starty + 1.0f + y); // y
	        textureCoords.add(u1);
	        textureCoords.add(v1);
	        
	        // Right Top vertex
	        vertices.add(startx + 1.0f + x); // x
	        vertices.add(starty + y); // y
	        textureCoords.add(u1);
	        textureCoords.add(v0);
	        
	        // Left Top vertex
			vertices.add(startx + x); // x
	        vertices.add(starty + y); // y
	        textureCoords.add(u0);
	        textureCoords.add(v0);
	        
	        // Right Bottom vertex
	        vertices.add(startx + 1.0f + x); // x
	        vertices.add(starty + 1.0f + y); // y
	        textureCoords.add(u1);
	        textureCoords.add(v1);
            
            startx += 1.0f;
            if(startx == CHUNK_SIZE) {
            	startx = 0;
            	starty += 1.0f;
            }
		}
			
		vArr = MathUtils.listToArray(vertices);
		tcArr = MathUtils.listToArray(textureCoords);
	}
	
	private void initGL() {		
		vao = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vao);
		
		vVbo = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vVbo);
		FloatBuffer vBuffer = MemoryUtil.memAllocFloat(vArr.length);
		vBuffer.put(vArr).flip();
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, vBuffer, GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(0, 2, GL11.GL_FLOAT, false, 0, 0);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		
		tcVbo = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, tcVbo);
		FloatBuffer tcBuffer = MemoryUtil.memAllocFloat(tcArr.length);
		tcBuffer.put(tcArr).flip();
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, tcBuffer, GL15.GL_DYNAMIC_DRAW);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, 0, 0);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		
		MemoryUtil.memFree(vBuffer);
		MemoryUtil.memFree(tcBuffer);
		
		GL30.glBindVertexArray(0);
	}
	
	private void rebuild() {
		List<Float> textureCoords = new ArrayList<>();
		
		float startx = 0.0f;
		float starty = 0.0f;
		for(int i = 0; i < CHUNK_SIZE * CHUNK_SIZE; i++) {
			float u0 = ((tiles[(int) startx][(int) starty].getId() % (int)SPRITE_SHEET) / SPRITE_SHEET);
	        float u1 = (u0 + (1.0f / SPRITE_SHEET));
	        float v0 = ((tiles[(int) startx][(int) starty].getId() / (int)SPRITE_SHEET) / SPRITE_SHEET);
	        float v1 = (v0 + (1.0f / SPRITE_SHEET));
			
			// Left Top vertex
	        textureCoords.add(u0);
	        textureCoords.add(v0);
	        
	        // Left Bottom vertex
	        textureCoords.add(u0);
	        textureCoords.add(v1);
	        
	        // Right Bottom vertex
	        textureCoords.add(u1);
	        textureCoords.add(v1);
	        
	        // Right Top vertex
	        textureCoords.add(u1);
	        textureCoords.add(v0);
	        
	        // Left Top vertex
	        textureCoords.add(u0);
	        textureCoords.add(v0);
	        
	        // Right Bottom vertex
	        textureCoords.add(u1);
	        textureCoords.add(v1);
            
            startx += 1.0f;
            if(startx == CHUNK_SIZE) {
            	startx = 0;
            	starty += 1.0f;
            }
		}
			
		tcArr = MathUtils.listToArray(textureCoords);
		
		// Change texture coordinates buffer
		GL30.glBindVertexArray(vao);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, tcVbo);
		FloatBuffer buffer = MemoryUtil.memAllocFloat(tcArr.length);
		buffer.put(tcArr).flip();		
		GL15.glBufferSubData(GL15.GL_ARRAY_BUFFER, 0, buffer);
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
		
		MemoryUtil.memFree(buffer);
	}
	
	public void render() {
		if(rebuild) {
			rebuild();
			rebuild = false;
		}
		
		GL30.glBindVertexArray(vao);
		GL20.glEnableVertexAttribArray(0);
		GL20.glEnableVertexAttribArray(1);
		GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, vArr.length / 2);
		GL20.glDisableVertexAttribArray(0);
		GL20.glDisableVertexAttribArray(1);
		GL30.glBindVertexArray(0);
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}
	
	public Tile getTile(int x, int y) {
		return tiles[x][y];
	}
	
	public Terrain getTerrain() {
		return terrain;
	}
	
	public void setRebuild(boolean rebuild) {
		this.rebuild = rebuild;
	}
	
	public void cleanup() {
		GL20.glDisableVertexAttribArray(0);
		
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL15.glDeleteBuffers(vVbo);
		GL15.glDeleteBuffers(tcVbo);
		
		GL30.glBindVertexArray(0);
		GL30.glDeleteVertexArrays(vao);
	}
	
}
