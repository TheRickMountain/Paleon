package com.paleon.engine.graph.gui;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.RenderEngine;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Rect;
import com.paleon.scenes.Game;

public class Inventory {
	public int slotsX = 5, slotsY = 4;
	public List<Item> inventory = new ArrayList<Item>();
	public List<Item> slots = new ArrayList<Item>();
	private boolean showInventory;
	private boolean showTooltip;
	private String tooltip;
	
	private int slotSkin = ResourceManager.getTexture("ui_slot");
	
	private boolean draggingItem;
	private Item draggedItem;
	private int draggedIndex;
	
	GameObject tooltipText;
	
	public void init() 
	{
		for(int i = 0; i < (slotsX * slotsY); i++) 
		{
			slots.add(new Item());
			inventory.add(new Item());
		}
	}
	
	public void update() {
		if(Keyboard.isKeyDown(Key.TAB))
		{
			showInventory = !showInventory;
			
			if(showInventory) 
			{
				Mouse.show();
				Game.state = Game.State.INVENTORY;
			} 
			else 
			{
				Game.state = Game.State.GAME;
				Mouse.hide();
				showTooltip = false;
			}
		}
		
	}
	
	public void onGUI() 
	{
		tooltip = "";
		if(showInventory) 
		{
			drawInventory();
			if(showTooltip) {
				RenderEngine.renderGUI(new Rect(Mouse.getX() + 15f, Mouse.getY(), 200, 200), slotSkin);
			}
		}
		
		if(draggingItem)
		{
			RenderEngine.renderGUI(new Rect(Mouse.getX() - 25f, Mouse.getY() - 25f, 50, 50), draggedItem.itemIcon);
		}
	}
	
	private void drawInventory() 
	{	
		int i = 0;
		for(int y = 0; y < slotsY; y++) 
		{
			for(int x = 0; x < slotsX; x++) 
			{
				Rect rect = new Rect((Display.getWidth() / 2) + (x * 60), (Display.getHeight() / 3)  + (y * 60), 50, 50);
				
				RenderEngine.renderGUI(rect, slotSkin);
				slots.set(i, inventory.get(i));
				Item item = slots.get(i);
				if(slots.get(i).itemName != null) 
				{
					RenderEngine.renderGUI(rect, slots.get(i).itemIcon);
					if(Mouse.getX() >= rect.x && Mouse.getX() <= rect.x + rect.width &&
							Mouse.getY() >= rect.y && Mouse.getY() <= rect.y + rect.height) 
					{
						tooltip = generateTooltip(slots.get(i));
						showTooltip = true;
						if(Mouse.isButton(0) && !draggingItem)
						{
							draggingItem = true;
							draggedItem = item;
							inventory.set(i, new Item());
							draggedIndex = i;
						}
						if(Mouse.isButtonUp(0) && draggingItem)
						{
							inventory.set(draggedIndex, inventory.get(i));
							inventory.set(i, draggedItem);
							draggingItem = false;
							draggedItem = null;
						}
						if(Mouse.isButtonDown(1) && !draggingItem)
						{
							if(item.itemType == Item.ItemType.CONSUMABLE)
							{
								useConsumable(slots.get(i), i);
							}
						}
					}
				} else {
					if(Mouse.getX() >= rect.x && Mouse.getX() <= rect.x + rect.width &&
							Mouse.getY() >= rect.y && Mouse.getY() <= rect.y + rect.height) 
					{
						if(Mouse.isButtonUp(0) && draggingItem)
						{
							inventory.set(i, draggedItem);
							draggingItem = false;
							draggedItem = null;
						}
					}
				}
				
				if(tooltip == "")
				{
					showTooltip = false;
				}
				
				i++;
			}
		}
	}
	
	private String generateTooltip(Item item) {
		tooltip = item.itemName;
		return tooltip;
	}
	
	public void removeItem(int itemID) 
	{
		for(int i = 0; i < inventory.size(); i++) 
		{
			if(inventory.get(i).itemID == itemID) 
			{
				inventory.set(i, new Item());
				break;
			}
		}
	}
	
	private void useConsumable(Item item, int slot)
	{
		boolean deleteItem = false;
		switch(item.itemID)
		{
		case 1:
			if(Game.gui.hungerBar.getCurrentValue() != Game.gui.hungerBar.getMaxValue()) {
				Game.gui.hungerBar.increase(item.itemFood);
				deleteItem = true;
			}
			break;
		}
		
		if(deleteItem)
			inventory.set(slot, new Item());
	}
	
	public boolean addItem(int itemID) 
	{
		for(int i = 0; i < ItemDatabase.items.size(); i++) 
		{
			if(itemID == i)
			{
				for(int j = 0; j < slots.size(); j++)
				{
					if(inventory.get(j).itemName == null)
					{
						inventory.set(j, ItemDatabase.items.get(i));
						return true;
					}
				}
			}
		}
		return false;
	}
	
	public boolean inventoryContains(int id) 
	{
		for(Item item : inventory) 
		{
			if(item.itemID == id) return true;
		}
		return false;
	}
}
