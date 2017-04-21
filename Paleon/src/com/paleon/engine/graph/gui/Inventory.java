package com.paleon.engine.graph.gui;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.RenderEngine;
import com.paleon.engine.graph.renderSystems.GUIRendererSystem;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.toolbox.Color;
import com.paleon.engine.toolbox.Rect;
import com.paleon.scenes.Game;

public class Inventory 
{
	public int slotsX = 4, slotsY = 5;
	public List<Item> inventory = new ArrayList<Item>();
	public List<Item> slots = new ArrayList<Item>();
	private boolean showInventory;
	private boolean showTooltip;
	
	private String tooltip;
	private String food;
	private String weight;
	
	private int slotSkin = ResourceManager.getTexture("ui_slot");
	
	private boolean draggingItem;
	private Item draggedItem;
	private int draggedIndex;
	
	public boolean action = false;
	
	public void init() 
	{
		for(int i = 0; i < (slotsX * slotsY); i++) 
		{
			slots.add(new Item());
			inventory.add(new Item());
		}
		
		addItem(1);
		addItem(2);
		addItem(3);
		addItem(4);
		addItem(4);
		addItem(5);
		addItem(6);
		addItem(7);
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
	
	public void onGUI(Crafting crafting) 
	{
		tooltip = "";
		if(showInventory) 
		{
			action = false;
			RenderEngine.renderGUI(new Rect(Display.getWidth() / 2, Display.getHeight() / 3 - 10, 250, 310), slotSkin);
			drawInventory();
			if(showTooltip) 
			{
				RenderEngine.renderGUI(new Rect(Mouse.getX() + 15f, Mouse.getY(), 200, 200), slotSkin);
				RenderEngine.renderGUI(new Rect(Mouse.getX() + 25f, Mouse.getY() + 10, 50, 50), 
						new Text(tooltip, GUIRendererSystem.primitiveFont, 1.25f, Color.BLACK, 1f, false));
				RenderEngine.renderGUI(new Rect(Mouse.getX() + 25f, Mouse.getY() + 50, 50, 50), 
						new Text(food, GUIRendererSystem.primitiveFont, 1f, Color.BLUE, 1f, false));
				RenderEngine.renderGUI(new Rect(Mouse.getX() + 25f, Mouse.getY() + 70, 50, 50), 
						new Text(weight, GUIRendererSystem.primitiveFont, 1f, Color.PURPLE, 1f, false));
			}
			if(action) {
				crafting.updateListElements();
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
				Rect rect = new Rect((Display.getWidth() / 2) + (x * 60 + 10), (Display.getHeight() / 3)  + (y * 60), 50, 50);
				
				RenderEngine.renderGUI(rect, slotSkin);
				slots.set(i, inventory.get(i));
				Item item = slots.get(i);
				if(slots.get(i).itemName != null) 
				{
					RenderEngine.renderGUI(rect, slots.get(i).itemIcon);
					if(Mouse.getX() >= rect.x && Mouse.getX() <= rect.x + rect.width &&
							Mouse.getY() >= rect.y && Mouse.getY() <= rect.y + rect.height) 
					{
						generateTooltip(slots.get(i));
						showTooltip = true;
						if(Mouse.isButton(0) && !draggingItem)
						{
							draggingItem = true;
							draggedItem = item;
							inventory.set(i, new Item());
							draggedIndex = i;
							action = true;
						}
						if(Mouse.isButtonUp(0) && draggingItem)
						{
							inventory.set(draggedIndex, inventory.get(i));
							inventory.set(i, draggedItem);
							draggingItem = false;
							draggedItem = null;
							action = true;
						}
						if(Mouse.isButtonDown(1) && !draggingItem)
						{
							if(item.itemType == Item.ItemType.CONSUMABLE)
							{
								useConsumable(slots.get(i), i);
								action = true;
							}
						}
					}
				} 
				else 
				{
					if(Mouse.getX() >= rect.x && Mouse.getX() <= rect.x + rect.width &&
							Mouse.getY() >= rect.y && Mouse.getY() <= rect.y + rect.height) 
					{
						if(Mouse.isButtonUp(0) && draggingItem)
						{
							inventory.set(i, draggedItem);
							draggingItem = false;
							draggedItem = null;
							action = true;
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
	
	private void generateTooltip(Item item) 
	{
		tooltip = item.itemName;
		food = "Food: " + item.itemFood;
		weight = "Weight: " + item.itemWeight;
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
			if(Game.gui.hungerBar.getCurrentValue() != Game.gui.hungerBar.getMaxValue()) 
			{
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
	
	public int inventoryContainsCount(int id){
		int count = 0;
		
		for(Item item : inventory) 
		{
			if(item.itemID == id) {
				count++;
			}
		}
		
		return count;
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
