package com.paleon.engine.components;

import com.paleon.engine.items.GameObject;

public abstract class Component {

	public boolean enabled = true;
	
	protected GameObject gameObject;
	
	public void setGameObject(GameObject gameObject) {
		this.gameObject = gameObject;
	}
	
}
