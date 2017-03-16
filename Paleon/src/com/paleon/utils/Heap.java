package com.paleon.utils;

public class Heap<T extends IHeapItem<? super T>> {

	protected T[] items;
	protected int size;
	
	@SuppressWarnings("unchecked")
	public Heap(int maxHeapSize) {
		items = (T[])new IHeapItem[maxHeapSize];
		size = 0;
	}
	
	public void add(T item) {
		item.setHeapIndex(size);
		items[size] = item;
		sortUp(item);
		size++;
	}
	
	public T removeFirst() {
		T firstItem = items[0];
		size--;
		items[0] = items[size];
		items[0].setHeapIndex(0);
		sortDown(items[0]);
		return firstItem;
	}
	
	public void updateItem(T item) {
		sortUp(item);
	}
	
	public int getCount() {
		return size;
	}
	
	public boolean contains(T item) {
		T newItem = items[item.getHeapIndex()];
		
		if(newItem == null || item == null) {
			return false;
		}
		
		return newItem.equals(item);
	}
	
	private void sortDown(T item) {
		while(true) {
			int childIndexLeft = item.getHeapIndex() * 2 + 1;
			int childIndexRight = item.getHeapIndex() * 2 + 2;
			int swapIndex = 0;
			
			if(childIndexLeft < size) {
				swapIndex = childIndexLeft;
				
				if(childIndexRight < size) {
					if(items[childIndexLeft].compareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}
				
				if(item.compareTo(items[swapIndex]) < 0) {
					swap(item, items[swapIndex]);
				} else {
					return;
				}
				
			} else {
				return;
			}
		}
	}
	
	private void sortUp(T item) {
		int parentIndex = (item.getHeapIndex() - 1) / 2;
		
		while(true) {
			T parentItem = items[parentIndex];
			if(item.compareTo(parentItem) > 0) {
				 swap(item, parentItem);
			} else {
				break;
			}
			
			parentIndex = (item.getHeapIndex() - 1) / 2;
		}
	}
	
	private void swap(T itemA, T itemB) {
		items[itemA.getHeapIndex()] = itemB;
		items[itemB.getHeapIndex()] = itemA;
		int itemAIndex = itemA.getHeapIndex();
		itemA.setHeapIndex(itemB.getHeapIndex());
		itemB.setHeapIndex(itemAIndex);
	}
	
}

