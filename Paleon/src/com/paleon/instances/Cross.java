package com.paleon.instances;

import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Cross extends Entity {

	public Cross() {
		super(ResourceManager.getTexture("cross"));
		setTag("cross");
		setScale(0.35f);
	}

}
