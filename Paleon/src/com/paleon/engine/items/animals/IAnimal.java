package com.paleon.engine.items.animals;

import com.paleon.maths.vecmath.Vector3f;

public interface IAnimal {
	
	public void update(float deltaTime);
	
	public String getTag();
	
	public int getId();
	
	public Vector3f getPosition();
	
	public void decreaseHealth(int value);
	
	public void increaseHealth(int value);
	
	public int getHealth();

	public boolean isAttacked();

	public void setAttacked(boolean attacked);
	
}
