package com.paleon.engine.graph;

import com.paleon.engine.items.Entity;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;

public class Transform {
	
	private Matrix4f modelMatrix = new Matrix4f();
	
	public Vector3f lastPosition;
	private Vector3f lastRotation;
	private Vector3f lastScale;
	
	private Entity gameObject;
	
	public boolean transformed = false;
	
	public Transform(Entity gameObject) {
		this.gameObject = gameObject;
		
		lastPosition = new Vector3f();
		lastRotation = new Vector3f();
		lastScale = new Vector3f();
		MathUtils.getEulerModelMatrix(modelMatrix, gameObject.position, gameObject.rotation, gameObject.scale);
	}
	
	public void update() {
		transformed = false;
		
		// Position
		if(lastPosition.x != gameObject.position.x) {
			lastPosition.x = gameObject.position.x;
			transformed = true;
		}
		
		if(lastPosition.y != gameObject.position.y) {
			lastPosition.y = gameObject.position.y;
			transformed = true;
		}
		
		if(lastPosition.z != gameObject.position.z) {
			lastPosition.z = gameObject.position.z;
			transformed = true;
		}
		
		// Rotation
		if(lastRotation.x != gameObject.rotation.x) {
			lastRotation.x = gameObject.rotation.x;
			transformed = true;
		}
		
		if(lastRotation.y != gameObject.rotation.y) {
			lastRotation.y = gameObject.rotation.y;
			transformed = true;
		}
		
		if(lastRotation.z != gameObject.rotation.z) {
			lastRotation.z = gameObject.rotation.z;
			transformed = true;
		}
		
		// Scale
		if(lastScale.x != gameObject.scale.x) {
			lastScale.x = gameObject.scale.x;
			transformed = true;
		}
		
		if(lastScale.y != gameObject.scale.y) {
			lastScale.y = gameObject.scale.y;
			transformed = true;
		}
		
		if(lastScale.z != gameObject.scale.z) {
			lastScale.z = gameObject.scale.z;
			transformed = true;
		}
		
		if(transformed) {
			MathUtils.getEulerModelMatrix(modelMatrix, lastPosition, lastRotation, lastScale);
		}
	}
	
	public Transform translate(Vector3f translation) {
		this.gameObject.position.x += translation.x;
		this.gameObject.position.y += translation.y;
		this.gameObject.position.z += translation.z;
		return this;
	}
	
	public Transform translate(float x, float y, float z) {
		this.gameObject.position.x += x;
		this.gameObject.position.y += y;
		this.gameObject.position.z += z;
		return this;
	}
	
	public Transform rotate(Vector3f eulerAngles) {
		this.gameObject.rotation.x += eulerAngles.x;
		this.gameObject.rotation.y += eulerAngles.y;
		this.gameObject.rotation.z += eulerAngles.z;
		return this;
	}
	
	public Transform rotate(Vector3f axis, float angle) {
		this.gameObject.rotation.x += axis.x * angle;
		this.gameObject.rotation.y += axis.y * angle;
		this.gameObject.rotation.z += axis.z * angle;
		return this;
	}
	
	public Transform rotate(float xAngle, float yAngle, float zAngle) {
		this.gameObject.rotation.x += xAngle;
		this.gameObject.rotation.y += yAngle;
		this.gameObject.rotation.z += zAngle;
		return this;
	}
	
	public Matrix4f getModelMatrix() {
		return modelMatrix;
	}
	
}
