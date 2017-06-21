	package com.paleon.core;

import com.paleon.input.Mouse;
import com.paleon.math.Matrix4f;
import com.paleon.math.Vector3f;
import com.paleon.toolbox.MathUtils;

/**
 * Created by Rick on 07.10.2016.
 */
public class Camera {

    public static final float FOV = 60.0f;
    public static final float Z_NEAR = 0.1f;
    public static final float Z_FAR = 1000.f;

    private static final float MIN_DISTANCE = 0;
	private static final float MAX_DISTANCE = 40;
	private float distanceFromPlayer = 5;
	private float zoomSpeed = 15;
	
	 private static final float MAX_PITCH = 85;
	    private static final float MIN_PITCH = 0;
    private float angleAroundPlayer = 0;

    private final Vector3f position;

    private float pitch = 55;

    private float yaw = 0;

    public Vector3f playerPosition;
    
    private Matrix4f projectionMatrix;
    private Matrix4f viewMatrix;
    private Matrix4f projectionViewMatrix;

    public Camera(Vector3f playerPosition) {
        this.playerPosition = playerPosition;
        this.position = new Vector3f(0, 0, 0);
        
        this.projectionMatrix = new Matrix4f();
        this.viewMatrix = new Matrix4f();
        
        updateProjectionMatrix();
    }

    public Vector3f getPosition() {
        return position;
    }

    public void invertPitch() {
    	this.pitch = -pitch;
    }
    
    public void update(float dt) {
    	rotate(dt);
    	updateViewMatrix();
    	
    	if(Display.isResized()) {
    		updateProjectionMatrix();
    	}
    }

    private void rotate(float dt) {
    	if(Mouse.isButtonDown(2)) {
            Mouse.hide();
        }

        if(Mouse.isButtonUp(2)) {
            Mouse.show();
        }
    	
    	if(Mouse.isButton(2)) {
            calculateAngleAroundPlayer(dt);
            calculatePitch(dt);
    	} 
        calculateZoom(dt);

        float horizontalDistance = calculateHorizontalDistance();
        float verticalDistance = calculateVerticalDistance();
        calculateCameraPosition(horizontalDistance, verticalDistance);
        this.yaw = 180 - angleAroundPlayer;
    }
    
    public float getPitch() {
        return pitch;
    }

    public float getYaw() {
        return yaw;
    }

    private void calculateAngleAroundPlayer(float dt){
        float angleChange = Mouse.Cursor.getDX() * 20 * dt;
        angleAroundPlayer -= angleChange;
    }

    private void calculatePitch(float dt){
        float pitchChange = -Mouse.Cursor.getDY() * 10 * dt;
        pitch -= pitchChange;
        if(pitch >= MAX_PITCH){
            pitch = MAX_PITCH;
        } else if(pitch <= MIN_PITCH){
            pitch = MIN_PITCH;
        }
    }

    private void calculateCameraPosition(float horizDistance, float verticDistance){
        float theta = angleAroundPlayer;
        float offsetX = (float) (horizDistance * Math.sin(Math.toRadians(theta)));
        float offsetZ = (float) (horizDistance * Math.cos(Math.toRadians(theta)));
        position.x = playerPosition.x - offsetX;
        position.z = playerPosition.z - offsetZ;
        position.y = playerPosition.y + verticDistance;
    }

    private float calculateHorizontalDistance(){
        return (float) (distanceFromPlayer * Math.cos(Math.toRadians(pitch)));
    }

    private float calculateVerticalDistance(){
        return (float) (distanceFromPlayer * Math.sin(Math.toRadians(pitch)));
    }

    private void calculateZoom(float dt){
        float zoomLevel = Mouse.getScroll() * dt * zoomSpeed;
        float temp = distanceFromPlayer;
        temp -= zoomLevel;
        if(temp >= MAX_DISTANCE){
            temp = MAX_DISTANCE;
        } else if(temp <= MIN_DISTANCE){
            temp = MIN_DISTANCE;
        }
        distanceFromPlayer = temp;
    }

    public void updateProjectionMatrix() {
    	MathUtils.getProjectionMatrix(projectionMatrix, FOV, Display.getWidth(), Display.getHeight(), 
				Z_NEAR, Z_FAR);
    }
    
    public void updateViewMatrix() {
    	MathUtils.getViewMatrix(viewMatrix, this);
    }
    
	public Matrix4f getProjectionMatrix() {
		return projectionMatrix;
	}

	public Matrix4f getViewMatrix() {
		return viewMatrix;
	}
	
	public Matrix4f getProjectionViewMatrix() {
		return Matrix4f.mul(projectionMatrix, viewMatrix, projectionViewMatrix);
	}
    
}
