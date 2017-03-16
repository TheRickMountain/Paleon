package com.paleon.instances;

import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.core.ResourceManager;
import com.paleon.ecs.Entity;

public class Pine extends Entity {

	public Pine() {
		super(ResourceManager.getTexture("pine"));
		setTag("tree");
		addComponent(new InfoComponent(this, InfoType.TREE));
		setMovementCost(0);
	}

}
