package com.paleon.instances;

import com.paleon.core.ResourceManager;

public class Stone extends Entity {

	public Stone() {
		super(ResourceManager.getTexture("stone"));
		setWalkable(false);
	}

}
