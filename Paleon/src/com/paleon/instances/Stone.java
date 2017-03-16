package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Stone extends Entity {

	public Stone() {
		super(ResourceManager.getTexture("stone"));
		setTag("stone");
		addComponent(new InfoComponent(this, InfoType.ROCK));
		setMovementCost(0.0f);
	}

}
