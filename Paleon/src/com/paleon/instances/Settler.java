package com.paleon.instances;

import com.paleon.components.SettlerComponent;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;
import com.paleon.terrain.Tile;

public class Settler extends Entity {

	public Settler(Tile tile) {
		super(ResourceManager.getTexture("settler"));
		setTag("settler");
		addComponent(new SettlerComponent(tile, this));
	}

	
	
}
