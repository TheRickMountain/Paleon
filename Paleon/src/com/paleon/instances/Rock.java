package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Rock extends Entity {

	public Rock() {
		super(ResourceManager.getTexture("rock"));
		setTag("rock");
		addComponent(new InfoComponent(this, InfoType.RESOURCE));
	}

}
