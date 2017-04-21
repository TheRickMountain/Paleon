package com.paleon.engine.terrain;

import com.paleon.engine.toolbox.MathUtils;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector3f;

public class TerrainBlock {

	private static final LODIndicesVBOsInfo[] INDEX_BUFFERS = createIndicesVBOs();

	private float x, z;
	private float size;
	
	private boolean highlight = false;
	private boolean selected = false;

	private float relativeX, relativeZ, relativeSize;

	private Terrain terrain;
	private int index;
	
	private float disFromCamera;

	private int lod = 0;

	private boolean stitchTop = false;
	private boolean stitchBottom = false;
	private boolean stitchLeft = false;
	private boolean stitchRight = false;
	
	private float[][] heights;

	protected TerrainBlock(int iteration, int nodeIndex, TerrainVertex[][] table, Terrain terrain,
			float[] vertices, float[] normals, float[] textureCoords) {
		this.index = nodeIndex;
		this.terrain = terrain;
		calculatePosition(iteration, nodeIndex);
		addToVAO(table, vertices, normals, textureCoords);
		extractHeights(table);
	}

	public Terrain getTerrain() {
		return terrain;
	}
	
	public void setStitching(){
		terrain.setStitching(this);
	}
	
	public boolean isHighlighted(){
		return highlight;
	}
	
	public void highlight(boolean high){
		this.highlight = high;
	}
	
	public void select(boolean select){
		this.selected = select;
	}
	
	public boolean isSelected(){
		return selected;
	}

	public int[] getIndicesVBOInfo() {
		return INDEX_BUFFERS[lod].getIndexVBO(stitchTop, stitchBottom, stitchLeft, stitchRight);
	}

	public int[] getIndicesVBOInfo(int forcedLOD) {
		// terrain.setStitching(this);
		if (forcedLOD > lod) {
			return INDEX_BUFFERS[forcedLOD].getIndexVBO(false, false, false, false);
		}else{
			return INDEX_BUFFERS[lod].getIndexVBO(false, false, false, false);
		}
	}

	public void setLOD(int lod) {
		this.lod = lod;
	}

	public float getX() {
		return x;
	}

	public float getZ() {
		return z;
	}

	public float getSize() {
		return size;
	}

	public int getIndex() {
		return index;
	}
	
	public void setDisFromCamera(float distance){
		this.disFromCamera = distance;
	}

	public float getDistanceFromCamera(){
		return disFromCamera;
	}
	
	public float calcHeight(float terrainXPos, float terrainZPos) {
		float gridSquareSize = size / ((float) heights.length - 1);
		int tempX = (int) Math.floor(terrainXPos / gridSquareSize);
		int tempZ = (int) Math.floor(terrainZPos / gridSquareSize);
		if (tempX == heights.length - 1) {
			tempX = heights.length - 2;
		}
		if (tempZ == heights.length - 1) {
			tempZ = heights.length - 2;
		}
		float xCoord = (terrainXPos % gridSquareSize) / gridSquareSize;
		float zCoord = (terrainZPos % gridSquareSize) / gridSquareSize;
		float answer;
		if (zCoord >= xCoord) {
			answer = MathUtils
					.barryCentric(new Vector3f(0, heights[tempZ][tempX], 0), new Vector3f(1,
							heights[tempZ + 1][tempX], 0), new Vector3f(1,
							heights[tempZ + 1][tempX + 1], 1), new Vector2f(zCoord, xCoord));
		} else {
			answer = MathUtils
					.barryCentric(new Vector3f(0, heights[tempZ][tempX], 0), new Vector3f(0,
							heights[tempZ][tempX + 1], 1), new Vector3f(1,
							heights[tempZ + 1][tempX + 1], 1), new Vector2f(zCoord, xCoord));
		}
		return answer;
	}

	public Vector3f calculateTerrainNormal(float terrainXPos, float terrainZPos) {
		float gridSquareSize = Terrain.SIZE / (float) heights.length - 1;
		int tempX = (int) Math.floor(terrainXPos / gridSquareSize);
		int tempZ = (int) Math.floor(terrainZPos / gridSquareSize);
		if (tempX == heights.length) {
			tempX = heights.length;
		}
		if (tempZ == heights.length) {
			tempZ = heights.length;
		}
		float xCoord = terrainXPos % gridSquareSize;
		float zCoord = terrainZPos % gridSquareSize;
		Vector3f point00 = new Vector3f(0, heights[tempX][tempZ], 0);
		Vector3f point11 = new Vector3f(gridSquareSize, heights[tempX + 1][tempZ + 1],
				gridSquareSize);
		if (xCoord > zCoord) {
			Vector3f point10 = new Vector3f(gridSquareSize, heights[tempX + 1][tempZ], 0);
			return MathUtils.calculateNormal(point00, point11, point10);
		} else {
			Vector3f point01 = new Vector3f(0, heights[tempX][tempZ + 1], gridSquareSize);
			return MathUtils.calculateNormal(point00, point01, point11);
		}
	}

	protected int getLOD() {
		return lod;
	}

	protected void setStitchTop(boolean stitchTop) {
		this.stitchTop = stitchTop;
	}

	protected void setStitchBottom(boolean stitchBottom) {
		this.stitchBottom = stitchBottom;
	}

	protected void setStitchLeft(boolean stitchLeft) {
		this.stitchLeft = stitchLeft;
	}

	protected void setStitchRight(boolean stitchRight) {
		this.stitchRight = stitchRight;
	}

	private void calculatePosition(int iteration, int nodeIndex) {
		int iterationSize = (int) Math.pow(2, iteration);
		int row = nodeIndex / iterationSize;
		int col = nodeIndex % iterationSize;
		float fraction = 1f / iterationSize;
		relativeX = col * fraction;
		relativeZ = row * fraction;
		relativeSize = 1f / iterationSize;
		this.size = relativeSize * Terrain.SIZE;
		this.x = terrain.getX() + (relativeX * Terrain.SIZE) + size / 2;
		this.z = terrain.getZ() + (relativeZ * Terrain.SIZE) + size / 2;
		//System.out.println(x + " " + z);
	}

	private void addToVAO(TerrainVertex[][] table, float[] vertices, float[] normals,
			float[] textCoords) {
		float vertexDistance = size / (table.length - 1);
		int numVertices = table.length * table.length;
		int vertexPointer = numVertices * 3 * index;
		int normalPointer = numVertices * 3 * index;
		int texturePointer = numVertices * 2 * index;
		for (int i = 0; i < table.length; i++) {
			for (int j = 0; j < table[i].length; j++) {
				Vector3f normal = table[i][j].getNormal();
				vertices[vertexPointer++] = (relativeX * Terrain.SIZE) + (vertexDistance * j);
				vertices[vertexPointer++] = table[i][j].getHeight();
				vertices[vertexPointer++] = (vertexDistance * i) + (relativeZ * Terrain.SIZE);
				normals[normalPointer++] = normal.x;
				normals[normalPointer++] = normal.y;
				normals[normalPointer++] = normal.z;
				textCoords[texturePointer++] = relativeX
						+ ((relativeSize / (table.length - 1)) * j);
				textCoords[texturePointer++] = relativeZ
						+ ((relativeSize / (table.length - 1)) * i);
			}
		}
	}

	private static LODIndicesVBOsInfo[] createIndicesVBOs() {
		LODIndicesVBOsInfo[] indicesVBOs = new LODIndicesVBOsInfo[Terrain.NUM_OF_LODS];
		for (int lod = 0; lod < indicesVBOs.length; lod++) {
			indicesVBOs[lod] = new LODIndicesVBOsInfo(lod, Terrain.BLOCK_WIDTH);
		}
		return indicesVBOs;
	}

	private void extractHeights(TerrainVertex[][] table) {
		heights = new float[table.length][table.length];
		for (int i = 0; i < heights.length; i++) {
			for (int j = 0; j < heights[i].length; j++) {
				heights[i][j] = table[i][j].getHeight();
			}
		}
	}

}
