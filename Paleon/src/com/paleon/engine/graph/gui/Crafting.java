package com.paleon.engine.graph.gui;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.behaviour.ButtonBh;
import com.paleon.engine.components.Image;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;
import com.paleon.scenes.World;

public class Crafting {

	private boolean showCrafting = false;
	
	private int slotSkin = ResourceManager.getTexture("ui_slot");
	
	private World world;
	private Inventory inventory;
	
	private List<ListElement> listElements;
	
	GameObject background;
	
	CraftingResult craftingResult;
	
	Button craftButton;
	
	Item activeItem;
	
	int scrollValue = 0;
	
	int listElementsSize = 7;
	
	public void init(World world, Inventory inventory) {
		this.world = world;
		this.inventory = inventory;
		
		listElements = new ArrayList<ListElement>();
		
		background = new GameObject();
		background.addComponent(new Image(slotSkin, new Color(1f, 1f, 1f)));
		background.position.x = Display.getWidth() / 2 - 410;
		background.position.y = Display.getHeight() / 3 - 10;
		background.scale.x = 410;
		background.scale.y = 310;
		background.getComponent(Image.class).enabled = false;
		world.addGameObject(background);
		
		craftingResult = new CraftingResult(ItemDatabase.items.get(5));
		craftingResult.position.x = background.position.x + 170;
		craftingResult.position.y = background.position.y + 10;
		craftingResult.setActive(false);
		world.addGameObject(craftingResult);
		
		craftButton = (Button) craftingResult.getChildByName("Button");
	}
	
	public void update() {		
		if(Keyboard.isKeyDown(Key.TAB)) {
			showCrafting = !showCrafting;
			
			if(showCrafting) {
				background.getComponent(Image.class).enabled = true;	
			
				updateListElements();
				
			} else {
				background.getComponent(Image.class).enabled = false;
				
				for(ListElement lE : listElements) {
					world.removeGameObject(lE);
				}
				listElements.clear();
				
				craftingResult.setActive(false);
			}
		}
		
		if(showCrafting) {
			if(Mouse.isButtonDown(0)) {
				for(ListElement lL : listElements) {
					if(lL.getBehaviour(ButtonBh.class).isPressedDown(0)) {
						activeItem = lL.getItem();
						craftingResult.setActive(true);
						craftingResult.setItem(activeItem);
					}
				}
				
				if(craftButton.getBehaviour(ButtonBh.class).isPressedDown(0)) {
					inventory.addItem(activeItem.itemID);
				}
			}
			
			int scroll = (int) Mouse.getScroll();
			if(scroll != 0) {
				if(scroll > 0) {
					if(!(scrollValue >= 0)) {
						scrollValue++;
						updateListElements();
					}
				} else if(scroll < 0){
					int temp = -(listElements.size() - listElementsSize);
					if(!(scrollValue <= temp)) {
						scrollValue--;
						updateListElements();
					}
				}
			}

		}
	}
	
	public void updateListElements() {
		for(ListElement lL : listElements) {
			world.removeGameObject(lL);
		}
		listElements.clear();
		
		addListElement(1, scrollValue);
		addListElement(2, scrollValue + 1);
		addListElement(3, scrollValue + 2);
		addListElement(4, scrollValue + 3);
		addListElement(5, scrollValue + 4);
		addListElement(6, scrollValue + 5);
		addListElement(7, scrollValue + 6);
		addListElement(1, scrollValue + 7);
		addListElement(2, scrollValue + 8);
		
		for(ListElement lL : listElements) {
			world.addGameObject(lL, true);
		}
	}
	
	private void addListElement(int itemId, int offset) {
		ListElement lE = new ListElement(ItemDatabase.items.get(itemId));
		lE.position.x = background.position.x + 10;
		lE.position.y = background.position.y + 15 + (lE.height * offset);
		if(offset <= -1) {
			lE.setActive(false);
		} else if(offset >= listElementsSize) {
			lE.setActive(false);
		}
		listElements.add(lE);
	}
	
}
