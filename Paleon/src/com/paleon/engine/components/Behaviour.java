package com.paleon.engine.components;

import com.paleon.engine.items.GameObject;

public abstract class Behaviour {
	
	protected GameObject gameObject;
	
	public void setGameObject(GameObject gameObject) {
		this.gameObject = gameObject;
	}
	
	public abstract void create();
	
	public abstract void update(float deltaTime);
	
	public abstract void onGui();
	
}
