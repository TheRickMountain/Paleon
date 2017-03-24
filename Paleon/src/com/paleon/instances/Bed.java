package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Bed extends Entity  {
	
	public Bed() {
		super(ResourceManager.getTexture("bed"));
		setTag("bed");
		addComponent(new InfoComponent(this, InfoType.FURNITURE, null));
	}

}
