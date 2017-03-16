package com.paleon.components;

import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;

public class InfoComponent extends Component {
	
	private InfoType type;
	
	public InfoComponent(Entity parent, InfoType type) {
		super(parent);
		this.type = type;
	}

	@Override
	public void update(float dt) {
		
	}

	public InfoType getInfoType() {
		return type;
	}

	@Override
	public ComponentType getType() {
		return ComponentType.INFO;
	}

}
