package com.paleon.engine.behaviour;

import com.paleon.engine.components.Behaviour;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.items.Camera;
import com.paleon.scenes.World;

public class CharacterControllerBh extends Behaviour {

	private Camera camera;
	private World world;
	private boolean moving = false;
	
	private float speed = 8;
	
	private CharacterAnimBh playerAnim;
	
	public CharacterControllerBh(Camera camera, World world) {
		this.camera = camera;
		this.world = world;
	}
	
	@Override
	public void create() {
		playerAnim = gameObject.getBehaviour(CharacterAnimBh.class);
	}

	@Override
	public void update(float dt) {
		moving = false;
		
		gameObject.position.y = world.getTerrainHeight(gameObject.position.x, gameObject.position.z) + 2.25f;
		
		camera.setPosition(gameObject.position.x, gameObject.position.y + 4.5f, gameObject.position.z);
		
		/*if(Keyboard.isKeyDown(Key.E)) {
			if(world.getColorPickedObject() != null) {
				if(inventory.addItem(world.getColorPickedObject().getGuiId())) {
					world.getColorPickedObject().remove();
				}
			}
		}*/
		
		float tempRotation = -camera.getYaw();
		
		if(Keyboard.isKey(Key.W) && Keyboard.isKey(Key.A)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 45)) * -1.0f * speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 45)) * speed * dt;
			gameObject.rotation.y = tempRotation + 45;
			moving = true;
		} else if(Keyboard.isKey(Key.W) && Keyboard.isKey(Key.D)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 45)) * -1.0f * speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 45)) * speed * dt;
			gameObject.rotation.y = tempRotation -  45;
			moving = true;
		} else if(Keyboard.isKey(Key.S) && Keyboard.isKey(Key.D)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 45)) * -1.0f * -speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 45)) * -speed * dt;
			gameObject.rotation.y = tempRotation - 135;
			moving = true;
		} else if(Keyboard.isKey(Key.S) && Keyboard.isKey(Key.A)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 45)) * -1.0f * -speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 45)) * -speed * dt;
			gameObject.rotation.y = tempRotation + 135;
			moving = true;
		} else if(Keyboard.isKey(Key.W)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation)) * -1.0f * speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation)) * speed * dt;
			gameObject.rotation.y = tempRotation;
			moving = true;
		} else if(Keyboard.isKey(Key.S)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation)) * -1.0f * -speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation)) * -speed * dt;
			gameObject.rotation.y = tempRotation - 180;
			moving = true;
		} else if(Keyboard.isKey(Key.A)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation - 90)) * -1.0f * speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation - 90)) * speed * dt;
			gameObject.rotation.y = tempRotation + 90;
			moving = true;
		} else if(Keyboard.isKey(Key.D)) {
			gameObject.position.x -= (float)Math.sin(Math.toRadians(-tempRotation + 90)) * -1.0f * speed * dt;
			gameObject.position.z -= (float)Math.cos(Math.toRadians(-tempRotation + 90)) * speed * dt;
			gameObject.rotation.y = tempRotation - 90;
			moving = true;
		}
		
		if(moving) {
			speed = 8;
			if(Keyboard.isKey(Key.LEFT_SHIFT)) {
				speed = 20; 
				playerAnim.runAnim(dt);
			} else {
				playerAnim.walkAnim(dt);
			}
		} else {
			playerAnim.idleAnim(dt);
		}
	}

	@Override
	public void onGui() {
		// TODO Auto-generated method stub
		
	}

}
