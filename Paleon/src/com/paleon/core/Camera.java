package com.paleon.core;

import com.paleon.input.Key;
import com.paleon.input.Keyboard;
import com.paleon.input.Mouse;
import com.paleon.math.Matrix4f;
import com.paleon.utils.MathUtils;

public class Camera {

	private float x;
	private float y;
	
	private Matrix4f projectionMatrix = new Matrix4f();
	private Matrix4f viewMatrix = new Matrix4f();
	
	private float lastZoom = 0.0f;
	private float zoom = 0.025f;
	
	private float lastFrameX;
	private float lastFrameY;
	
	private float speed = 12f;
	
	public Camera() {
		updateProjectionMatrix();
	}
	
	public void update(float dt) {
		moveKeyboard(dt);
		moveMouse(dt);
		
		float scroll = Mouse.getScroll();
		if(scroll != 0) {
			zoom -= scroll * dt * 0.1f;
		}
		
		if(Display.isResized() || (lastZoom != zoom)) {
			updateProjectionMatrix();
			lastZoom = zoom;
		}
		MathUtils.getViewMatrix(viewMatrix, this);
	}
	
	private void moveKeyboard(float dt) {
		int upKey = Keyboard.isKey(Key.KEY_W) ? 1 : 0;
		int downKey = Keyboard.isKey(Key.KEY_S) ? 1 : 0;
		int leftKey = Keyboard.isKey(Key.KEY_A) ? 1 : 0;
		int rightKey = Keyboard.isKey(Key.KEY_D) ? 1 : 0;
		
		int xaxis = (rightKey - leftKey);
		int yaxis = (downKey - upKey);
		
		float direction = MathUtils.getDirection(0, 0, xaxis, yaxis);
		float length = 0;
	
		if(xaxis != 0 || yaxis != 0) {
			length = speed * dt;
		}
	
		x += MathUtils.getLengthdirX(length, direction);
		y += MathUtils.getLengthdirY(length, direction);
	}
	
	private void moveMouse(float dt) {
		float currFrameX = Mouse.getX();
		float currFrameY = Mouse.getY();
		
		if(Mouse.isButton(2)) {
			float diffx = lastFrameX - currFrameX;
			float diffy = lastFrameY - currFrameY;
			
			x += (diffx * zoom * 60) * dt;
			y += (diffy * zoom * 60) * dt;
		}
		
		lastFrameX = Mouse.getX();
		lastFrameY = Mouse.getY();
	}
	
	public float getX() {
		return x;
	}

	public float getY() {
		return y;
	}
	
	public void setPosition(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public Matrix4f getProjectionMatrix() {
		return projectionMatrix;
	}
	
	public Matrix4f getViewMatrix() {
		return viewMatrix;
	}
	
	public float getZoom() {
		return zoom;
	}
	
	private void updateProjectionMatrix() {
		MathUtils.getOrthoProjectionMatrix(projectionMatrix, 
				0, Display.getWidth() * zoom, Display.getHeight() * zoom, 0);
	}
	
}
