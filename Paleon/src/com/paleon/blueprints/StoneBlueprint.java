package com.paleon.blueprints;

import com.paleon.instances.Entity;
import com.paleon.instances.Stone;

public class StoneBlueprint implements Blueprint {
	
	public Entity getInstance() {
		return new Stone();
	}

}
