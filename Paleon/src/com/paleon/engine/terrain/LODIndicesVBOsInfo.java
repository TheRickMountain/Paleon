package com.paleon.engine.terrain;

import java.util.ArrayList;
import java.util.List;

public class LODIndicesVBOsInfo {// TODO could be written to file to avoid
									// calculation every load


	private int[] vbos;
	private int[] lengths;
	private int nodeSize;

	protected LODIndicesVBOsInfo(int lOD, int nodeSize) {
		this.nodeSize = nodeSize;
		vbos = new int[16];
		lengths = new int[16];

		for (int i = 0; i < vbos.length; i++) {
			boolean top = i % 2 >= 1;
			boolean bottom = i % 4 >= 2;
			boolean left = i % 8 >= 4;
			boolean right = i % 16 >= 8;
			int[] indices = getIndices(lOD, top, bottom, left, right);
			vbos[i] = Loader.createIndicesVBO(indices);
			lengths[i] = indices.length;
		}

	}

	protected int[] getIndexVBO(boolean top, boolean bottom, boolean left, boolean right) {
		int index = 0;
		if (right) {
			index += 8;
		}
		if (left) {
			index += 4;
		}
		if (bottom) {
			index += 2;
		}
		if (top) {
			index += 1;
		}
		return new int[] { vbos[index], lengths[index] };
	}

	private int[] getIndices(int lod, boolean top, boolean bottom, boolean left, boolean right) {
		int numberPerRow = nodeSize + 1;
		int twoToPowerOfLod = (int) Math.pow(2, lod);
		List<Integer> indices = new ArrayList<Integer>();
		int actualNodeSize = nodeSize / twoToPowerOfLod;
		if (actualNodeSize == 1) {
			int topLeft = 0;
			int topRight = topLeft + twoToPowerOfLod;
			int bottomLeft = topLeft + (numberPerRow * twoToPowerOfLod);
			int bottomRight = bottomLeft + twoToPowerOfLod;
			return new int[] { topLeft, bottomLeft, bottomRight, bottomRight, topRight, topLeft };
		}
		for (int y = 1; y < actualNodeSize - 1; y++) {
			for (int x = 1; x < actualNodeSize - 1; x++) {
				int topLeft = (numberPerRow * y * twoToPowerOfLod) + (x * twoToPowerOfLod);
				int topRight = topLeft + twoToPowerOfLod;
				int bottomLeft = topLeft + (numberPerRow * twoToPowerOfLod);
				int bottomRight = bottomLeft + twoToPowerOfLod;
				indices.add(topLeft);
				indices.add(bottomLeft);
				indices.add(bottomRight);
				indices.add(bottomRight);
				indices.add(topRight);
				indices.add(topLeft);
			}
		}
		List<Integer> topRow = getTopRow(top, numberPerRow, twoToPowerOfLod, actualNodeSize);
		for (Integer newInt : topRow) {
			indices.add(newInt);
		}
		List<Integer> bottomRow = getBottomRow(bottom, numberPerRow, twoToPowerOfLod,
				actualNodeSize);
		for (Integer newInt : bottomRow) {
			indices.add(newInt);
		}
		List<Integer> leftRow = getLeftRow(left, numberPerRow, twoToPowerOfLod, actualNodeSize);
		for (Integer newInt : leftRow) {
			indices.add(newInt);
		}
		List<Integer> rightRow = getRightRow(right, numberPerRow, twoToPowerOfLod, actualNodeSize);
		for (Integer newInt : rightRow) {
			indices.add(newInt);
		}

		int[] indicesArray = new int[indices.size()];
		for (int i = 0; i < indicesArray.length; i++) {
			indicesArray[i] = indices.get(i);
		}
		return indicesArray;
	}

	private static List<Integer> getTopRow(boolean top, int numberPerRow, int twoToPowerOfLod,
			int actualNodeSize) {
		List<Integer> indices = new ArrayList<Integer>();
		for (int x = 0; x < actualNodeSize; x += 2) {
			int topLeft = x * twoToPowerOfLod;
			int topMiddle = topLeft + twoToPowerOfLod;
			int topRight = topMiddle + twoToPowerOfLod;
			int bottomLeft = topLeft + (numberPerRow * twoToPowerOfLod);
			int bottomMiddle = bottomLeft + twoToPowerOfLod;
			int bottomRight = bottomMiddle + twoToPowerOfLod;
			indices.add(topLeft);
			indices.add(bottomLeft);
			indices.add(bottomMiddle);
			indices.add(topLeft);
			indices.add(bottomMiddle);
			if (!top) {
				indices.add(topMiddle);
				indices.add(topMiddle);
				indices.add(bottomMiddle);
			}
			indices.add(topRight);
			indices.add(topRight);
			indices.add(bottomMiddle);
			indices.add(bottomRight);
		}
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(0);
		indices.remove(0);
		indices.remove(0);
		return indices;
	}

	private static List<Integer> getBottomRow(boolean bottom, int numberPerRow,
			int twoToPowerOfLod, int actualNodeSize) {
		List<Integer> indices = new ArrayList<Integer>();
		for (int x = 0; x < actualNodeSize; x += 2) {
			int topLeft = (numberPerRow * (actualNodeSize - 1) * twoToPowerOfLod)
					+ (x * twoToPowerOfLod);
			int topMiddle = topLeft + twoToPowerOfLod;
			int topRight = topMiddle + twoToPowerOfLod;
			int bottomLeft = topLeft + (numberPerRow * twoToPowerOfLod);
			int bottomMiddle = bottomLeft + twoToPowerOfLod;
			int bottomRight = bottomMiddle + twoToPowerOfLod;
			indices.add(topLeft);
			indices.add(bottomLeft);
			indices.add(topMiddle);
			indices.add(topMiddle);
			indices.add(bottomLeft);
			if (!bottom) {
				indices.add(bottomMiddle);
				indices.add(topMiddle);
				indices.add(bottomMiddle);
			}
			indices.add(bottomRight);
			indices.add(bottomRight);
			indices.add(topRight);
			indices.add(topMiddle);
		}
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(0);
		indices.remove(0);
		indices.remove(0);
		return indices;
	}

	private static List<Integer> getLeftRow(boolean left, int numberPerRow, int twoToPowerOfLod,
			int actualNodeSize) {
		List<Integer> indices = new ArrayList<Integer>();
		for (int y = 0; y < actualNodeSize; y += 2) {
			int topLeft = (numberPerRow * y * twoToPowerOfLod);
			int topRight = topLeft + twoToPowerOfLod;
			int middleLeft = topLeft + (numberPerRow * twoToPowerOfLod);
			int middleRight = middleLeft + twoToPowerOfLod;
			int bottomLeft = middleLeft + (numberPerRow * twoToPowerOfLod);
			int bottomRight = bottomLeft + twoToPowerOfLod;
			indices.add(topLeft);
			indices.add(middleRight);
			indices.add(topRight);
			indices.add(middleRight);
			indices.add(topLeft);
			if (!left) {
				indices.add(middleLeft);
				indices.add(middleRight);
				indices.add(middleLeft);
			}
			indices.add(bottomLeft);
			indices.add(bottomLeft);
			indices.add(bottomRight);
			indices.add(middleRight);
		}
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(0);
		indices.remove(0);
		indices.remove(0);
		return indices;
	}

	private static List<Integer> getRightRow(boolean right, int numberPerRow, int twoToPowerOfLod,
			int actualNodeSize) {
		List<Integer> indices = new ArrayList<Integer>();
		for (int y = 0; y < actualNodeSize; y += 2) {
			int topLeft = (numberPerRow * y * twoToPowerOfLod)
					+ ((actualNodeSize - 1) * twoToPowerOfLod);
			int topRight = topLeft + twoToPowerOfLod;
			int middleLeft = topLeft + (numberPerRow * twoToPowerOfLod);
			int middleRight = middleLeft + twoToPowerOfLod;
			int bottomLeft = middleLeft + (numberPerRow * twoToPowerOfLod);
			int bottomRight = bottomLeft + twoToPowerOfLod;
			indices.add(topLeft);
			indices.add(middleLeft);
			indices.add(topRight);
			indices.add(topRight);
			indices.add(middleLeft);
			if (!right) {
				indices.add(middleRight);
				indices.add(middleRight);
				indices.add(middleLeft);
			}
			indices.add(bottomRight);
			indices.add(bottomRight);
			indices.add(middleLeft);
			indices.add(bottomLeft);
		}
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(indices.size() - 1);
		indices.remove(0);
		indices.remove(0);
		indices.remove(0);
		return indices;
	}

}
