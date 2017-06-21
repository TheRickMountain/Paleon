package com.paleon.ecs;

public class Transform {
	
	private Entity parent;
	
	public float x = 0;
	public float y = 0;
	public float z = 0;
	public float rotX = 0;
	public float rotY = 0;
	public float rotZ = 0;
	public float scaleX = 1;
	public float scaleY = 1;
	public float scaleZ = 1;
	
	public boolean isMoving;
	
	public Transform() {
		this(0, 0, 0, 0, 0, 0, 1);
	}
	
	public Transform(float x, float y, float z, float rotX, float rotY, float rotZ, float scale) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.rotX = rotX;
		this.rotY = rotY;
		this.rotZ = rotZ;
		this.scaleX = scale;
		this.scaleY = scale;
		this.scaleZ = scale;
	}
	
	protected void setParent(Entity parent) {
		this.parent = parent;
	}

	public Entity getParent() {
		return parent;
	}
	
	public Transform setPosition(float x, float y, float z){
		this.x = x;
		this.y = y;
		this.z = z;
		return this;
	}
	
	public Transform setRotation(float rotX, float rotY, float rotZ){
		this.rotX = rotX;
		this.rotY = rotY;
		this.rotZ = rotZ;
		return this;
	}
	
	public Transform setScale(float scale) {
		this.scaleX = scale;
		this.scaleY = scale;
		this.scaleZ = scale;
		return this;
	}
	
	public Transform setScale(float scaleX, float scaleY, float scaleZ){
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		this.scaleZ = scaleZ;
		return this;
	}

	
}
