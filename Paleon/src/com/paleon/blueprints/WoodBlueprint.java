package com.paleon.blueprints;

import com.paleon.ecs.Entity;
import com.paleon.instances.Wood;

public class WoodBlueprint implements Blueprint {
	
	public Entity getInstance() {
		return new Wood();
	}

}
