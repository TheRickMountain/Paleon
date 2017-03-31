package com.paleon.blueprints;

import com.paleon.instances.Entity;
import com.paleon.utils.Color;

public class EntityBlueprint implements Blueprint {

	public Entity getInstance() {
		return new Entity(new Color(1.0f, 1.0f, 1.0f, 1.0f));
	}
	
}
