package com.paleon.engine.items.animals;

import com.paleon.engine.items.Entity;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.TimeUtil;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.scenes.World;

public class Sheep implements IAnimal {

	private int maxRadius = 40;
	private int relaxTime = 5;
	
	private int health = 40;
	
	private Entity body, frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg;
	
	public int animSpeed = 100;
	
	private float frontLeftLegRotation = 0;
	private float frontRightLegRotation = 0;
	private boolean frontLegsState = false;
	
	public float currentRotation = 0;
	private float tempRotation = 0;
	
	private float walkingSpeed = -5;
	
	private Vector3f targetPosition;
	
	private TimeUtil timer, panicTime;
	private double currentTime;

	private int id = 0;
	
	private String tag = "Sheep";
	
	private boolean attacked = false;
	
	private World world;
	
	public Sheep(World world, float x, float y, float z) {
		this.world = world;
		
		body = SheepModel.getBody();
		body.scale.set(0.9f);
		frontLeftLeg = SheepModel.getFrontLeftLeg();
		frontRightLeg = SheepModel.getFrontRightLeg();
		backLeftLeg = SheepModel.getBackLeftLeg();
		backLeftLeg.scale.set(1.25f);
		backRightLeg = SheepModel.getBackRightLeg();
		backRightLeg.scale.set(1.25f);
		
		id = MathUtils.generateId();
		body.setId(id);
		frontLeftLeg.setId(id);
		frontRightLeg.setId(id);
		backLeftLeg.setId(id);
		backRightLeg.setId(id);
		
		setPosition(x, y, z);
		
		timer = new TimeUtil();
		currentTime = 0;
		
		panicTime = new TimeUtil();
	}
	
	public void update(float deltaTime) {
		if(targetPosition != null) {
			moveAnimation(deltaTime);
			translate(deltaTime);
			float distance = MathUtils.getDistanceBetweenPoints(body.position.x, body.position.z, targetPosition.x, targetPosition.z);
			if(distance <= 4f) {
				targetPosition = null;
				resetAnimation();
				timer.reset();
				currentTime = 0;
			}
		} else {
			currentTime = timer.getTime();
			if(currentTime >= relaxTime) {
				targetPosition = MathUtils.getRandomPointInCirle((int)body.position.x, (int)body.position.z, maxRadius);
				currentRotation = MathUtils.getAngle(targetPosition, body.position);
				rotate(currentRotation);
			}
		}
		
		if(attacked) {
			if(panicTime.getTime() >= 10){
				panicTime.reset();
				attacked = false;
				relaxTime = 5;
				animSpeed = 100;
				walkingSpeed = -5;
			} else {
				relaxTime = 0;
				animSpeed = 200;
				walkingSpeed = -10;
			}
		}
	}
	
	public void moveAnimation(float deltaTime){
		if(frontLegsState){
			frontLeftLegRotation += animSpeed * deltaTime;
			frontRightLegRotation -= animSpeed * deltaTime;
		} else {
			frontLeftLegRotation -= animSpeed * deltaTime;
			frontRightLegRotation += animSpeed * deltaTime;
		}
		
		if(frontLeftLegRotation >= 35) {
			frontLegsState = false;
		} else if(frontLeftLegRotation <= -35) {
			frontLegsState = true;
		}
		
		frontLeftLeg.rotation.x = frontLeftLegRotation;
		frontRightLeg.rotation.x = frontRightLegRotation;
		backLeftLeg.rotation.x = frontRightLegRotation;
		backRightLeg.rotation.x = frontLeftLegRotation;
	}
	
	public void resetAnimation(){
		frontLeftLeg.rotation.x = 0;
		frontRightLeg.rotation.x = 0;
		backLeftLeg.rotation.x = 0;
		backRightLeg.rotation.x = 0;
	}
	
	public void rotate(float rotation){
		rot(-tempRotation);
		rot(rotation);
		tempRotation = rotation;
	}
	
	float offsetX = 0;
	float offsetZ = 0;
	public void translate(float deltaTime){
		offsetX = (float)Math.sin(Math.toRadians(currentRotation)) * -1.0f * -walkingSpeed * deltaTime; 
		offsetZ = (float)Math.cos(Math.toRadians(currentRotation)) * -walkingSpeed * deltaTime;
		
		body.position.x += offsetX;
		frontLeftLeg.position.x += offsetX;
		frontRightLeg.position.x += offsetX;
		backLeftLeg.position.x += offsetX;
		backRightLeg.position.x += offsetX;
		
		body.position.z += offsetZ;
		frontLeftLeg.position.z += offsetZ;
		frontRightLeg.position.z += offsetZ;
		backLeftLeg.position.z += offsetZ;
		backRightLeg.position.z += offsetZ;
		
		setHeight(world.getTerrainHeight(body.position.x, body.position.z));
	}
	
	private void rot(float rotation){
		/*** Front Legs Y rotation ***/
		// Front left leg
		Vector2f frontLeftLegRotAround = MathUtils.getRotationAroundPoint(body.position, frontLeftLeg.position, 
				rotation * MathUtils.DEGREES_TO_RADIANS);
		frontLeftLeg.position.set(frontLeftLegRotAround.x, frontLeftLeg.position.y, frontLeftLegRotAround.y);
		frontLeftLeg.rotation.y = -MathUtils.getRotationBetweenPoints(frontLeftLeg.position, body.position) - 120;
		
		// Front right leg
		Vector2f frontRightLegRotAround = MathUtils.getRotationAroundPoint(body.position, frontRightLeg.position, 
				rotation * MathUtils.DEGREES_TO_RADIANS);
		frontRightLeg.position.set(frontRightLegRotAround.x, frontRightLeg.position.y, frontRightLegRotAround.y);
		frontRightLeg.rotation.y = -MathUtils.getRotationBetweenPoints(frontRightLeg.position, body.position) - 60;
		//*** *** ***//*
		
		//*** Back Legs Y rotation ***//*
		// Front left leg
		Vector2f backLeftLegRotAround = MathUtils.getRotationAroundPoint(body.position, backLeftLeg.position, 
				rotation * MathUtils.DEGREES_TO_RADIANS);
		backLeftLeg.position.set(backLeftLegRotAround.x, backLeftLeg.position.y, backLeftLegRotAround.y);
		backLeftLeg.position.y = -MathUtils.getRotationBetweenPoints(backLeftLeg.position, body.position) + 120;

		// Front right leg
		Vector2f backRightLegRotAround = MathUtils.getRotationAroundPoint(body.position, backRightLeg.position, 
				rotation * MathUtils.DEGREES_TO_RADIANS);
		backRightLeg.position.set(backRightLegRotAround.x, backRightLeg.position.y, backRightLegRotAround.y);
		backRightLeg.rotation.y = -MathUtils.getRotationBetweenPoints(backRightLeg.position, body.position) + 60;
		/*** *** ***/
		
		body.rotation.y = -MathUtils.getRotationBetweenPoints(frontLeftLeg.position, body.position) - 115;
	}
	
	public void add(World world) {
		world.addGameObject(body);
		world.addGameObject(frontLeftLeg);
		world.addGameObject(frontRightLeg);
		world.addGameObject(backLeftLeg);
		world.addGameObject(backRightLeg);
	}
	
	public void setCurrentRotation(float rot){
		currentRotation = rot;
		rotate(rot);
	}
	
	public void setPosition(float x, float y, float z){
		body.position.set(x, y + 0.2f, z);
		frontLeftLeg.position.set(x + 0.35f, y + 1.2f, z + 0.75f);
		frontRightLeg.position.set(x - 0.35f, y + 1.2f, z + 0.75f);
		backLeftLeg.position.set(x + 0.35f, y + 1.4f, z - 0.75f);
		backRightLeg.position.set(x - 0.35f, y + 1.4f, z - 0.75f);
	}
	
	public void setHeight(float value) {
		body.position.y = value + 0.2f;
		frontLeftLeg.position.y = value + 1.2f;
		frontRightLeg.position.y = value + 1.2f;
		backLeftLeg.position.y = value + 1.4f;
		backRightLeg.position.y = value + 1.4f;
	}
	
	public String getTag() {
		return tag;
	}

	public Vector3f getPosition() {
		return body.position;
	}
	
	@Override
	public int getId() {
		return body.getId();
	}

	@Override
	public void decreaseHealth(int value) {
		this.health -= value;
	}

	@Override
	public void increaseHealth(int value) {
		this.health += value;
	}

	@Override
	public int getHealth() {
		return health;
	}

	public boolean isAttacked() {
		return attacked;
	}

	public void setAttacked(boolean attacked) {
		this.attacked = attacked;
	}
	
}
