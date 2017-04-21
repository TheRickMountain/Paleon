package com.paleon.engine.items;

import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;

public class Player extends APlayer {

	public float speed = 0;
	
	private boolean moving = false;
	
	private float tempRotation = 0;
	
	public void move(float dt, Camera camera) {
		moving = false;
		
		currentRotation = -camera.getYaw();
		tempRotation = currentRotation;
		
		if(Keyboard.isKey(Key.W) && Keyboard.isKey(Key.A)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 45)) * -1.0f * speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 45)) * speed * dt;
			currentRotation = tempRotation + 45;
			moving = true;
		} else if(Keyboard.isKey(Key.W) && Keyboard.isKey(Key.D)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 45)) * -1.0f * speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 45)) * speed * dt;
			currentRotation = tempRotation -  45;
			moving = true;
		} else if(Keyboard.isKey(Key.S) && Keyboard.isKey(Key.D)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 45)) * -1.0f * -speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 45)) * -speed * dt;
			currentRotation = tempRotation - 135;
			moving = true;
		} else if(Keyboard.isKey(Key.S) && Keyboard.isKey(Key.A)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 45)) * -1.0f * -speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 45)) * -speed * dt;
			currentRotation = tempRotation + 135;
			moving = true;
		} else if(Keyboard.isKey(Key.W)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation)) * -1.0f * speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation)) * speed * dt;
			currentRotation = tempRotation;
			moving = true;
		} else if(Keyboard.isKey(Key.S)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation)) * -1.0f * -speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation)) * -speed * dt;
			currentRotation = tempRotation - 180;
			moving = true;
		} else if(Keyboard.isKey(Key.A)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 90)) * -1.0f * speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 90)) * speed * dt;
			currentRotation = tempRotation + 90;
			moving = true;
		} else if(Keyboard.isKey(Key.D)) {
			body.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 90)) * -1.0f * speed * dt;
			body.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 90)) * speed * dt;
			currentRotation = tempRotation - 90;
			moving = true;
		}
		
		if(moving) {
			speed = 8;
			if(Keyboard.isKey(Key.LEFT_SHIFT)) {
				runAnim(dt);
				speed = 20; 
			} else {
				walkAnim(dt);
			}
		} else {
			idleAnim(dt);
		}
	}
	
}
