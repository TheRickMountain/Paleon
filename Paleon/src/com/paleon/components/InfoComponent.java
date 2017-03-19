package com.paleon.components;

import com.paleon.blueprints.Blueprint;
import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;

public class InfoComponent extends Component {
	
	private InfoType type;
	private Blueprint productionResourceBlueprint;
	
	public InfoComponent(Entity parent, InfoType type, Blueprint productionResourceBlueprint) {
		super(parent);
		this.type = type;
		this.productionResourceBlueprint = productionResourceBlueprint;
	}

	@Override
	public void update(float dt) {
		
	}
	
	public InfoType getInfoType() {
		return type;
	}

	public Entity getProductionResource() {
		return productionResourceBlueprint.getInstance();
	}
	
	@Override
	public ComponentType getType() {
		return ComponentType.INFO;
	}

}
