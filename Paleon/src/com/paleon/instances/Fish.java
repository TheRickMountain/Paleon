package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Fish extends Entity {

	public Fish() {
		super(ResourceManager.getTexture("fish"));
		setTag("fish");
		addComponent(new InfoComponent(this, InfoType.GATHERING, null));
	}

}
