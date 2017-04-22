package com.paleon.engine.behaviour;

import com.paleon.engine.components.Behaviour;
import com.paleon.engine.items.Entity;

public class CharacterAnimBh extends Behaviour {

	private Entity leftArm, leftForearm, rightArm, rightForearm;
	private Entity leftHip, leftShin, rightHip, rightShin;
	
	private boolean extremitiesState = false;
	
	private int animSpeed = 200;
	
	public CharacterAnimBh() {
		
	}
	
	@Override
	public void create() {
		leftArm = gameObject.getChildByName("LeftArm");
		leftForearm = leftArm.getChildByName("LeftForearm");
		
		rightArm = gameObject.getChildByName("RightArm");
		rightForearm = rightArm.getChildByName("RightForearm");
		
		leftHip = gameObject.getChildByName("LeftHip");
		leftShin = leftHip.getChildByName("LeftShin");
		
		rightHip = gameObject.getChildByName("RightHip");
		rightShin = rightHip.getChildByName("RightShin");
	}

	@Override
	public void update(float deltaTime) {
		
	}
	
	public void walkAnim(float dt) {
		animSpeed = 125;
		
		if(leftArm.localRotation.x >= 40) {
			extremitiesState = true;
		} else if(leftArm.localRotation.x <= -40){
			extremitiesState = false;
		}
		
		if(extremitiesState) {
			leftArm.localRotation.x -= animSpeed * dt;
			rightArm.localRotation.x += animSpeed * dt;
			
			leftHip.localRotation.x += animSpeed * dt;
			rightHip.localRotation.x -= animSpeed * dt;
		} else {
			leftArm.localRotation.x += animSpeed * dt;
			rightArm.localRotation.x -= animSpeed * dt;
			
			leftHip.localRotation.x -= animSpeed * dt;
			rightHip.localRotation.x += animSpeed * dt;
		}
		
		leftForearm.localRotation.x = 45;
		rightForearm.localRotation.x = 45;
		
		leftShin.localRotation.x = -20;
		rightShin.localRotation.x = -20;
		
		animSpeed = 200;
	}
	
	public void runAnim(float dt) {
		if(leftArm.localRotation.x >= 60) {
			extremitiesState = true;
		} else if(leftArm.localRotation.x <= -60){
			extremitiesState = false;
		}
		
		if(extremitiesState) {
			leftArm.localRotation.x -= animSpeed * dt;
			rightArm.localRotation.x += animSpeed * dt;
			
			leftHip.localRotation.x += animSpeed * dt;
			rightHip.localRotation.x -= animSpeed * dt;
		} else {
			leftArm.localRotation.x += animSpeed * dt;
			rightArm.localRotation.x -= animSpeed * dt;
			
			leftHip.localRotation.x -= animSpeed * dt;
			rightHip.localRotation.x += animSpeed * dt;
		}
		
		leftForearm.localRotation.x = 70;
		rightForearm.localRotation.x = 70;
		
		leftShin.localRotation.x = -20;
		rightShin.localRotation.x = -20;
	}
	
	public void idleAnim(float dt) {
		resetRotationX(leftArm, dt);
		resetRotationX(leftForearm, dt);
		
		resetRotationX(rightArm, dt);
		resetRotationX(rightForearm, dt);
		
		resetRotationX(leftHip, dt);
		resetRotationX(leftShin, dt);
		
		resetRotationX(rightHip, dt);
		resetRotationX(rightShin, dt);
	}
	
	private void resetRotationX(Entity gameObject, float dt) {
		if(gameObject.localRotation.x > 2) {
			gameObject.localRotation.x -= animSpeed * dt;
		} else if(gameObject.localRotation.x < -2) {
			gameObject.localRotation.x += animSpeed * dt;
		} else {
			gameObject.localRotation.x = 0;
		}
	}

	@Override
	public void onGui() {
		// TODO Auto-generated method stub
		
	}

}
