package com.paleon.engine.graph.gui;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.behaviour.ButtonBh;
import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.renderSystems.GUIRendererSystem;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;

public class ListElement extends GameObject {

	private Item item;
	
	public int width = 150;
	public int height = 40;
	
	public ListElement(Item item) {
		this.item = item;
		GameObject background = new GameObject();
		background.addComponent(new Image(ResourceManager.getTexture("ui_button"), new Color(1f, 1f, 1f)));
		background.localScale.x = width;
		background.localScale.y = height;
		background.name = "Background";
		addChild(background);
		
		GameObject elementIcon = new GameObject();
		elementIcon.addComponent(new Image(item.itemIcon, new Color(1f, 1f, 1f)));
		elementIcon.localScale.x = 30;
		elementIcon.localScale.y = 30;
		elementIcon.localPosition.x = 4;
		elementIcon.localPosition.y = 4;
		addChild(elementIcon);
		
		GameObject elementName = new GameObject();
		elementName.addComponent(new Text(item.itemName, GUIRendererSystem.primitiveFont, 0.9f, new Color(1f, 1f, 1f), 1f, false));
		elementName.localPosition.x = 40;
		elementName.localPosition.y = 10;
		addChild(elementName);
		
		addComponent(new ButtonBh());
	}
	
	public Item getItem() {
		return item;
	}
	
}
