package com.paleon.engine.graph.gui;

import com.paleon.engine.ResourceManager;

public class Item {

	public String itemName;
	public int itemID;
	public String itemDesc;
	public int itemIcon;
	public int itemFood;
	public int itemSpeed;
	public ItemType itemType;
	
	public enum ItemType {
		WEAPON,
		CONSUMABLE,
		QUEST
	}
	
	public Item() {
		
	}

	public Item(String itemName, int itemId, String itemDesc, int itemFood, int itemSpeed,
			ItemType itemType) {
		this.itemName = itemName;
		this.itemID = itemId;
		this.itemDesc = itemDesc;
		this.itemIcon = ResourceManager.getTexture("ui_" + itemName);
		this.itemFood = itemFood;
		this.itemSpeed = itemSpeed;
		this.itemType = itemType;
	}
	
}
