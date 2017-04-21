package com.paleon.engine.items;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;
import com.paleon.scenes.World;


public abstract class APlayer {

	GameObject eyes;
	GameObject head;
	public GameObject body;
	GameObject leftArm;
	GameObject rightArm;
	GameObject leftForearm;
	GameObject rightForearm;
	GameObject leftHip;
	GameObject rightHip;
	GameObject leftShin;
	GameObject rightShin;
	
	private static final Vector4f headPos = new Vector4f(0, 0.85f, 0, 1.0f);
	
	private static final Vector4f rightArmPos = new Vector4f(0.65f, 0.6f, 0, 1.0f);
	private static final Vector4f leftArmPos = new Vector4f(-0.65f, 0.6f, 0, 1.0f);
	
	private static final Vector4f rightForearmPos = new Vector4f(1.08f, -0.7f, 0, 1.0f);
	private static final Vector4f leftForearmPos = new Vector4f(-1.08f, -0.7f, 0, 1.0f);
	
	private static final Vector4f leftHipPos = new Vector4f(-0.4f, -0.45f, 0, 1.0f);
	private static final Vector4f rightHipPos = new Vector4f(0.4f, -0.45f, 0, 1.0f);
	
	private static final Vector4f shinPos = new Vector4f(0, -0.8f, 0, 1.0f);
	
	private boolean armsLegsState = false;
	
	private int animSpeed = 200;
	
	protected float currentRotation = 0;
	
	public APlayer() {
		eyes = new GameObject();
		eyes.addComponent(new MeshFilter(ResourceManager.getMesh("eyes")));
		eyes.addComponent(new MeshRenderer());
		
		head = new GameObject();
		head.addComponent(new MeshFilter(ResourceManager.getMesh("head")));
		head.addComponent(new MeshRenderer());
		
		body = new GameObject();
		body.addComponent(new MeshFilter(ResourceManager.getMesh("body")));
		body.addComponent(new MeshRenderer());
		
		leftArm = new GameObject();
		leftArm.addComponent(new MeshFilter(ResourceManager.getMesh("rightArm")));
		leftArm.addComponent(new MeshRenderer());
		rightArm = new GameObject();
		rightArm.addComponent(new MeshFilter(ResourceManager.getMesh("leftArm")));
		rightArm.addComponent(new MeshRenderer());
		
		leftForearm = new GameObject();
		leftForearm.addComponent(new MeshFilter(ResourceManager.getMesh("rightForearm")));
		leftForearm.addComponent(new MeshRenderer());
		rightForearm = new GameObject();
		rightForearm.addComponent(new MeshFilter(ResourceManager.getMesh("leftForearm")));
		rightForearm.addComponent(new MeshRenderer());
		
		leftHip = new GameObject();
		leftHip.addComponent(new MeshFilter(ResourceManager.getMesh("hip")));
		leftHip.addComponent(new MeshRenderer());
		rightHip = new GameObject();
		rightHip.addComponent(new MeshFilter(ResourceManager.getMesh("hip")));
		rightHip.addComponent(new MeshRenderer());
		
		leftShin = new GameObject();
		leftShin.addComponent(new MeshFilter(ResourceManager.getMesh("shin")));
		leftShin.addComponent(new MeshRenderer());
		rightShin = new GameObject();
		rightShin.addComponent(new MeshFilter(ResourceManager.getMesh("shin")));
		rightShin.addComponent(new MeshRenderer());
	}
	
	public void update() {
		
		head.position = getObjectPosition(body, headPos);
		eyes.position.x = head.position.x;
		eyes.position.y = head.position.y - 1f;
		eyes.position.z = head.position.z;
		eyes.rotation.y = head.rotation.y + 180;
		
		leftArm.position = getObjectPosition(body, leftArmPos);
		rightArm.position = getObjectPosition(body, rightArmPos);
		leftArm.rotation.y = body.rotation.y;
		rightArm.rotation.y = body.rotation.y;
		
		leftForearm.position = getObjectPosition(leftArm, leftForearmPos);
		rightForearm.position = getObjectPosition(rightArm, rightForearmPos);
		leftForearm.rotation.y = body.rotation.y;
		rightForearm.rotation.y = body.rotation.y;
		
		leftHip.position = getObjectPosition(body, leftHipPos);
		rightHip.position = getObjectPosition(body, rightHipPos);
		leftHip.rotation.y = body.rotation.y;
		rightHip.rotation.y = body.rotation.y;
		
		leftShin.position = getObjectPosition(leftHip, shinPos);
		rightShin.position = getObjectPosition(rightHip, shinPos);
		leftShin.rotation.y = body.rotation.y;
		rightShin.rotation.y = body.rotation.y;
	}
	
	public void add(World world) {
		world.addGameObject(head);
		world.addGameObject(eyes);
		world.addGameObject(body);
		world.addGameObject(leftArm);
		world.addGameObject(rightArm);
		world.addGameObject(leftForearm);
		world.addGameObject(rightForearm);
		world.addGameObject(leftHip);
		world.addGameObject(rightHip);
		world.addGameObject(leftShin);
		world.addGameObject(rightShin);
	}
	
	public void setPosition(float x, float y, float z) {
		body.position.set(x, y + 1.1f, z);
	}
	
	public void setPosY(float y) {
		this.body.position.y = y + 1.1f;
	}
	
	public float getPosX() {
		return body.position.x;
	}
	
	public float getPosZ() {
		return body.position.z;
	}
	
	public float getPosY() {
		return body.position.y;
	}
	
	public Vector3f getPosition() {
		return body.position;
	}
	
	public void setScale(float scale) {
		head.scale.set(scale);
		eyes.scale.set(scale);
		body.scale.set(scale);
		leftArm.scale.set(scale);
		rightArm.scale.set(scale);
		leftForearm.scale.set(scale);
		rightForearm.scale.set(scale);
		leftHip.scale.set(scale);
		rightHip.scale.set(scale);
		leftShin.scale.set(scale);
		rightShin.scale.set(scale);
	}
	
	private Vector3f getObjectPosition(GameObject gameObject, Vector4f constantPos) {
		Matrix4f modelMatrix = MathUtils.getEulerModelMatrix(gameObject.position, gameObject.rotation, gameObject.scale);
		Vector4f newPos =  Matrix4f.transform(modelMatrix, constantPos, null);
		return new Vector3f(newPos.x, newPos.y, newPos.z);
	}
	
	protected void runAnim(float dt) {
		if(leftArm.rotation.x >= 60) {
			armsLegsState = true;
		} else if(leftArm.rotation.x <= -60){
			armsLegsState = false;
		}
		
		if(armsLegsState) {
			leftArm.rotation.x -= animSpeed * dt;
			rightArm.rotation.x += animSpeed * dt;
			
			leftHip.rotation.x += animSpeed * dt;
			rightHip.rotation.x -= animSpeed * dt;
		} else {
			leftArm.rotation.x += animSpeed * dt;
			rightArm.rotation.x -= animSpeed * dt;
			
			leftHip.rotation.x -= animSpeed * dt;
			rightHip.rotation.x += animSpeed * dt;
		}
		
		leftForearm.rotation.x = leftArm.rotation.x + 70;
		rightForearm.rotation.x = rightArm.rotation.x + 70;
		
		leftShin.rotation.x = leftHip.rotation.x - 20;
		rightShin.rotation.x = rightHip.rotation.x - 20;
		
		body.rotation.y = currentRotation;
		head.rotation.y = currentRotation;
	}

	protected void walkAnim(float dt) {
		animSpeed = 125;
		
		if(leftArm.rotation.x >= 40) {
			armsLegsState = true;
		} else if(leftArm.rotation.x <= -40){
			armsLegsState = false;
		}
		
		if(armsLegsState) {
			leftArm.rotation.x -= animSpeed * dt;
			rightArm.rotation.x += animSpeed * dt;
			
			leftHip.rotation.x += animSpeed * dt;
			rightHip.rotation.x -= animSpeed * dt;
		} else {
			leftArm.rotation.x += animSpeed * dt;
			rightArm.rotation.x -= animSpeed * dt;
			
			leftHip.rotation.x -= animSpeed * dt;
			rightHip.rotation.x += animSpeed * dt;
		}
		
		leftForearm.rotation.x = leftArm.rotation.x + 45;
		rightForearm.rotation.x = rightArm.rotation.x + 45;
		
		leftShin.rotation.x = leftHip.rotation.x - 20;
		rightShin.rotation.x = rightHip.rotation.x - 20;
		
		body.rotation.y = currentRotation;
		head.rotation.y = currentRotation;
		
		animSpeed = 200;
	}
	
	protected void idleAnim(float dt) {
		resetRotationX(leftArm, dt);
		resetRotationX(leftForearm, dt);
		
		resetRotationX(rightArm, dt);
		resetRotationX(rightForearm, dt);
		
		resetRotationX(leftHip, dt);
		resetRotationX(leftShin, dt);
		
		resetRotationX(rightHip, dt);
		resetRotationX(rightShin, dt);
		
		body.rotation.y = currentRotation;
		head.rotation.y = currentRotation;
	}
	
	private void resetRotationX(GameObject gameObject, float dt) {
		if(gameObject.rotation.x > 2) {
			gameObject.rotation.x -= animSpeed * dt;
		} else if(gameObject.rotation.x < -2) {
			gameObject.rotation.x += animSpeed * dt;
		} else {
			gameObject.rotation.x = 0;
		}
	}
	
	public void clear() {
		body.getComponent(MeshFilter.class).mesh.cleanup();
	}
	
}
