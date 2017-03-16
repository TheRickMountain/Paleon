package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Wheat extends Entity {

	public Wheat() {
		super(ResourceManager.getTexture("wheat"));
		setTag("wheat");
		addComponent(new InfoComponent(this, InfoType.RESOURCE));
	}

}
