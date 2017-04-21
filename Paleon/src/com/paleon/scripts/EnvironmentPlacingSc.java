package com.paleon.scripts;

import com.paleon.engine.components.Component;
import com.paleon.engine.components.Script;
import com.paleon.engine.items.GameObject;

public class EnvironmentPlacingSc extends Component implements Script {

	public GameObject gameObject;
	public int count;
	
	@Override
	public void init() {
		for(int i = 0; i < count; i++) {
			
		}
	}

	@Override
	public void update() {
		
	}

}
