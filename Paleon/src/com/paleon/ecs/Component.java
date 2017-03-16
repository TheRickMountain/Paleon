package com.paleon.ecs;

public abstract class Component {

	protected Entity parent;
	public abstract void update(float dt);
	public abstract ComponentType getType();
	
	public Component(Entity parent) {
		this.parent = parent;
	}
	
	public Entity getParent() {
		return parent;
	}
	
}
