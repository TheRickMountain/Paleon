package com.paleon.instances;

import com.paleon.core.ResourceManager;

public class Pine extends Entity {

	public Pine() {
		super(ResourceManager.getTexture("pine"));
		setWalkable(false);
	}

}
