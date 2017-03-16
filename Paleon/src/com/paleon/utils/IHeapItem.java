package com.paleon.utils;

public abstract class IHeapItem<T> implements Comparable<T> {
	
	public abstract int getHeapIndex();
	public abstract void setHeapIndex(int heapIndex);
	
}
