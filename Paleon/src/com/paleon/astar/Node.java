package com.paleon.astar;

import com.paleon.utils.IHeapItem;
import com.paleon.utils.MathUtils;

public class Node<T> extends IHeapItem<Node<T>> {

	public T data;
	
	public int gCost;
	public int hCost;
	
	public Node<T> parent;
	
	private int heapIndex;
	
	public int getfCost() {
		return gCost + hCost;
	}

	@Override
	public int compareTo(Node<T> nodeToCompare) {
		int compare = MathUtils.compareTo(getfCost(), nodeToCompare.getfCost());
		if(compare == 0) {
			compare = MathUtils.compareTo(hCost, nodeToCompare.hCost);
		}
		return -compare;
		
	}

	@Override
	public int getHeapIndex() {
		return heapIndex;
	}

	@Override
	public void setHeapIndex(int heapIndex) {
		this.heapIndex = heapIndex;
	}
	
}
