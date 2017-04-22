package com.paleon.engine.components;

import com.paleon.engine.items.Entity;

public abstract class Behaviour {
	
	protected Entity gameObject;
	
	public void setGameObject(Entity gameObject) {
		this.gameObject = gameObject;
	}
	
	public abstract void create();
	
	public abstract void update(float deltaTime);
	
	public abstract void onGui();
	
}
