package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Wood extends Entity {

	public Wood() {
		super(ResourceManager.getTexture("wood"));
		setTag("wood");
		addComponent(new InfoComponent(this, InfoType.GATHERING, null));
	}

}
