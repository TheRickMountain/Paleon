package com.paleon.engine.graph.gui;

import java.util.ArrayList;
import java.util.List;

public class ItemDatabase {

	public static List<Item> items = new ArrayList<Item>();
	
	public static void init() {
		items.add(new Item("flint", 0, "Flint Desc", 0, 0, Item.ItemType.QUEST));
		items.add(new Item("shroom", 1, "Shroom Desc", 15, 0, Item.ItemType.CONSUMABLE));
		items.add(new Item("stick", 2, "Stick Desc", 0, 0, Item.ItemType.QUEST));
		items.add(new Item("wheat", 3, "Wheat Desc", 0, 0, Item.ItemType.QUEST));
	}
	
}
