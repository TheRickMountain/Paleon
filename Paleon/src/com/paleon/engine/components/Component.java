package com.paleon.engine.components;

import com.paleon.engine.items.Entity;

public abstract class Component {

	public boolean enabled = true;
	
	protected Entity gameObject;
	
	public void setEntity(Entity entity) {
		this.gameObject = entity;
	}
	
}
