package com.paleon.engine.graph.gui;

import java.io.File;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.font.FontType;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;
import com.paleon.scenes.World;

public class GUI {
	
	private Inventory inventory = new Inventory();
	
	private GameObject cross;
	
	public Bar healthBar;
	
	public Bar hungerBar;
	
	public void init(World world) {
		ItemDatabase.init();
		
		inventory.init();
		
		cross = new GameObject();
		cross.addComponent(new Image(ResourceManager.getTexture("ui_cross"), new Color(1.0f, 1.0f, 1.0f)));
		cross.position.set((Display.getWidth() / 2) - 20, (Display.getHeight() / 2) - 20);
		cross.scale.set(35, 35);
		world.addGameObject(cross);
		
		/*** Health bar ***/
		GameObject healthBarBack = new GameObject();
		healthBarBack.addComponent(new Image(ResourceManager.getTexture("bar_back"), new Color(1.0f, 1.0f, 1.0f)));
		healthBarBack.position.set(4, 8);
		healthBarBack.scale.set(165, 30);
		world.addGameObject(healthBarBack);
		
		GameObject health = new GameObject();
		health.addComponent(new Image(0, new Color(1.0f, 0.0f, 0.0f)));
		health.position.set(25, 15);
		health.scale.set(125, 15);
		world.addGameObject(health);
		healthBar = new Bar(health);
		healthBar.decrease(50);
		
		GameObject healthBarFront = new GameObject();
		healthBarFront.addComponent(new Image(ResourceManager.getTexture("bar_front"), new Color(1.0f, 1.0f, 1.0f)));
		healthBarFront.position.set(4, 8);
		healthBarFront.scale.set(165, 30);
		world.addGameObject(healthBarFront);
		/*** *** ***/
		
		FontType font = new FontType(ResourceManager.getTexture("primitive_font"), new File("res/primitive_font.fnt"));
		
		GameObject currentHealthText = new GameObject();
		currentHealthText.addComponent(new Text((int)healthBar.getCurrentValue() + "", 
				font, 1.15f, new Color(1f, 1f, 1f), 1f, false));
		currentHealthText.position.set(60, 10.5f);
		world.addGameObject(currentHealthText);
		
		GameObject maxHealthText = new GameObject();
		maxHealthText.addComponent(new Text((int)healthBar.getMaxValue() + "", 
				font, 1.15f, new Color(1f, 1f, 1f), 1f, false));
		maxHealthText.position.set(98, 10.5f);
		world.addGameObject(maxHealthText);
		
		/*** Hunger bar ***/
		GameObject hungerBarBack = new GameObject();
		hungerBarBack.addComponent(new Image(ResourceManager.getTexture("bar_back"), new Color(1.0f, 1.0f, 1.0f)));
		hungerBarBack.position.set(4, 43);
		hungerBarBack.scale.set(165, 30);
		world.addGameObject(hungerBarBack);
		
		GameObject hunger = new GameObject();
		hunger.addComponent(new Image(0, new Color(255, 165, 0)));
		hunger.position.set(25, 50);
		hunger.scale.set(125, 15);
		world.addGameObject(hunger);
		hungerBar = new Bar(hunger);
		hungerBar.decrease(50);
		
		GameObject hungerBarFront = new GameObject();
		hungerBarFront.addComponent(new Image(ResourceManager.getTexture("bar_front"), new Color(1.0f, 1.0f, 1.0f)));
		hungerBarFront.position.set(4, 43);
		hungerBarFront.scale.set(165, 30);
		world.addGameObject(hungerBarFront);
		/*** *** ***/
	}
	
	public void update() {
		inventory.update();
		
		if(healthBar.getCurrentValue() < healthBar.getMaxValue()) {
			if(hungerBar.getCurrentValue() == 100) {
				healthBar.increase(0.1f);
			}
		}
	}
	
	public void render() {
		if(Display.wasResized()) {
			cross.position.set((Display.getWidth() / 2) - 20, (Display.getHeight() / 2) - 20);
		}
		
		inventory.onGUI();
	}
	
	public Inventory getInventory() {
		return inventory;
	}
	
}
