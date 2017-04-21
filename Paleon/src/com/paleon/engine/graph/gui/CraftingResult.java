package com.paleon.engine.graph.gui;

import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.renderSystems.GUIRendererSystem;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;

public class CraftingResult extends GameObject {

	GameObject icon;
	GameObject name;
	
	public CraftingResult(Item item) {
		icon = new GameObject();
		icon.addComponent(new Image(item.itemIcon, new Color(1f, 1f, 1f)));
		icon.localScale.set(80);
		addChild(icon);
		
		name = new GameObject();
		name.addComponent(new Text(item.itemName, GUIRendererSystem.primitiveFont, 1.15f,
				new Color(0f, 0f, 0f), 1f, false));
		name.localPosition.x = icon.localScale.x + 10;
		addChild(name);
		
		Button button = new Button(100, 30, "Craft");
		button.name = "Button";
		button.localPosition.set( icon.localScale.x + 20, 250);
		addChild(button);
	}
	
	public void setItem(Item item) {
		icon.getComponent(Image.class).textureId = item.itemIcon;
		name.getComponent(Text.class).setText(item.itemName);
	}
	
}
