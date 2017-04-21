package com.paleon.engine.graph.gui;

import java.util.ArrayList;
import java.util.List;

public class ItemDatabase {

	public static List<Item> items = new ArrayList<Item>();
	
	public static void init() {
		items.add(new Item("", 0, "", 0, 0, Item.ItemType.QUEST));
		// Items
		items.add(new Item("shroom", 1, "shroom desc", 15, 0.1f, Item.ItemType.CONSUMABLE));
		items.add(new Item("stick", 2, "stick desc", 0, 0.3f, Item.ItemType.QUEST));
		items.add(new Item("wheat", 3, "wheat desc", 0, 0.25f, Item.ItemType.QUEST));
		items.add(new Item("flint", 4, "flint desc", 0, 0.5f, Item.ItemType.QUEST));
		items.add(new Item("sharp stone", 5, "sharp stone desc", 0, 0.4f, Item.ItemType.WEAPON));
		items.add(new Item("apple", 6, "apple desc", 10, 0.25f, Item.ItemType.CONSUMABLE));
		items.add(new Item("flour", 7, "flour desc", 0, 0.1f, Item.ItemType.QUEST));
	}
	
}
