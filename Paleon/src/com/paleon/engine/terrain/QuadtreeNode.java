package com.paleon.engine.terrain;

public class QuadtreeNode {

	private QuadtreeNode[][] children;

	private int index;
	private boolean isLeaf;
	private TerrainBlock terrainBlock;

	protected QuadtreeNode(int iteration, int nodeIndex, TerrainVertex[][] table, Terrain terrain,
			float[] vertices, float[] normals, float[] textureCoords) {
		this.index = nodeIndex;
		determineIfLeaf(table.length);
		if (!isLeaf) {
			createChildren(table, iteration, nodeIndex, terrain, vertices, normals, textureCoords);
		} else {
			terrainBlock = new TerrainBlock(iteration, nodeIndex, table, terrain, vertices,
					normals, textureCoords);
		}
	}

	public TerrainBlock getTerrainBlock() {
		return terrainBlock;
	}

	protected void addAllLeavesToList(TerrainBlock[] list) {
		if (isLeaf) {
			list[index] = terrainBlock;
		} else {
			for (int i = 0; i < children.length; i++) {
				for (int j = 0; j < children[i].length; j++) {
					children[i][j].addAllLeavesToList(list);
				}
			}
		}
	}

	private void determineIfLeaf(int tableSize) {
		if ((tableSize - 1) <= Terrain.BLOCK_WIDTH) {
			this.isLeaf = true;
		} else {
			this.isLeaf = false;
		}
	}

	private void createChildren(TerrainVertex[][] table, int currentInteration, int nodeIndex,
			Terrain terrain, float[] vertices, float[] normals, float[] texCoords) {
		children = new QuadtreeNode[2][2];
		for (int i = 0; i < children.length; i++) {
			for (int j = 0; j < children[i].length; j++) {
				TerrainVertex[][] childTable = createChildTable(i, j, table);
				children[i][j] = new QuadtreeNode(currentInteration + 1, calculateChildNodeIndex(
						currentInteration, nodeIndex, i, j), childTable, terrain, vertices,
						normals, texCoords);
			}
		}
	}

	private TerrainVertex[][] createChildTable(int i, int j, TerrainVertex[][] parentTable) {
		int halfSize = ((parentTable.length - 1) / 2);
		int childTableSize = halfSize + 1;
		TerrainVertex[][] childTable = new TerrainVertex[childTableSize][childTableSize];
		int startX = halfSize * i;
		int startY = halfSize * j;
		for (int x = 0; x < childTable.length; x++) {
			for (int y = 0; y < childTable[x].length; y++) {
				childTable[x][y] = parentTable[startX + x][startY + y];
			}
		}
		return childTable;
	}

	private int calculateChildNodeIndex(int iteration, int nodeIndex, int i, int j) {
		int iterationSize = (int) Math.pow(2, iteration);
		int row = (nodeIndex / iterationSize);
		int col = (nodeIndex % iterationSize);
		int index = (row * 4 * iterationSize) + (col * 2);
		index += j;
		index += (i * iterationSize * 2);
		return index;
	}
}
