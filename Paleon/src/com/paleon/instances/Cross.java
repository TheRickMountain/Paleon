package com.paleon.instances;

import com.paleon.core.ResourceManager;

public class Cross extends Entity {

	public Cross() {
		super(ResourceManager.getTexture("cross"));
		setScale(0.35f);
	}

}
