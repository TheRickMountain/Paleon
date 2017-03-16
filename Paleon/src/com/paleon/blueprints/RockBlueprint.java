package com.paleon.blueprints;

import com.paleon.ecs.Entity;
import com.paleon.instances.Rock;

public class RockBlueprint implements Blueprint {
	
	public Entity getInstance() {
		return new Rock();
	}

}
