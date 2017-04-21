package com.paleon.engine.toolbox;

import com.paleon.maths.vecmath.Vector3f;

public class Plane {

	private Triangle triangle;
	private Vector3f origin;
	private Vector3f normal;
	private float constant;
	
	private float t0;
	private float t1;

	public Plane(Triangle triangle) {
		this.triangle = triangle;
		Vector3f point0 = triangle.getPointN(0);
		Vector3f point1 = triangle.getPointN(1);
		Vector3f point2 = triangle.getPointN(2);
		normal = Vector3f.cross(Vector3f.sub(point2, point0, null),
				Vector3f.sub(point1, point0, null), null);
		normal.normalise();
		origin = new Vector3f(point0.x,point0.y,point0.z);
		constant = -(normal.x*origin.x+normal.y*origin.y+normal.z*origin.z);
	}
	
	public Plane(Vector3f normal,Vector3f origin){
		this.normal = normal;
		this.origin = origin;
		constant = -(normal.x*origin.x+normal.y*origin.y+normal.z*origin.z);
	}
	
	public float getSignedDistance(Vector3f point){
		return Vector3f.dot(normal, point)+constant;
	}
	
	public boolean isFrontFacingTo(Vector3f direction){
		float dot = Vector3f.dot(normal, direction);
		return (dot<=0);
	}

	public Triangle getTriangle() {
		return triangle;
	}

	public Vector3f getOrigin() {
		return origin;
	}
	
	public Vector3f getNormal() {
		return normal;
	}

	public float getConstant() {
		return constant;
	}

	public float getT0() {
		return t0;
	}

	public void setT0(float t0) {
		this.t0 = t0;
	}

	public float getT1() {
		return t1;
	}

	public void setT1(float t1) {
		this.t1 = t1;
	}
	
}
