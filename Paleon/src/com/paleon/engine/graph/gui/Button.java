package com.paleon.engine.graph.gui;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.behaviour.ButtonBh;
import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.renderSystems.GUIRendererSystem;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.Color;

public class Button extends GameObject {

	public Button(int width, int height, String text) {
		GameObject background = new GameObject();
		background.addComponent(new Image(ResourceManager.getTexture("ui_button"), new Color(1f, 1f, 1f)));
		background.localScale.x = width;
		background.localScale.y = height;
		background.name = "Background";
		addChild(background);
		
		if(!text.isEmpty()) {
			GameObject elementName = new GameObject();
			elementName.addComponent(new Text(text, GUIRendererSystem.primitiveFont, 0.9f, new Color(1f, 1f, 1f), 1f, false));
			elementName.localPosition.x = 25;
			elementName.localPosition.y = 5;
			addChild(elementName);
		}
		
		addComponent(new ButtonBh());
	}
	
}
