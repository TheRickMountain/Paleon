package com.paleon.engine.toolbox;

import com.paleon.maths.vecmath.Matrix3f;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class Triangle {

	private Vector3f[] points;

	public Triangle(Vector3f p0, Vector3f p1, Vector3f p2) {
		points = new Vector3f[3];
		points[0] = p0;
		points[1] = p1;
		points[2] = p2;
	}

	public Triangle getTransformedCopy(Matrix3f transform) {
		Vector3f[] newPoints = new Vector3f[3];
		for (int i = 0; i < newPoints.length; i++) {
			Vector3f point = points[i];
			Vector3f newPoint = new Vector3f();
			Matrix3f.transform(transform, point, newPoint);
			newPoints[i] = newPoint;
		}
		return new Triangle(newPoints[0], newPoints[1], newPoints[2]);
	}

	public Vector3f getPointN(int n) {
		return points[n];
	}
	
	public Triangle createInstance(Matrix4f transformation){
		Vector3f[] newPoints = new Vector3f[3];
		for(int i=0;i<points.length;i++){
			Vector3f original = points[i];
			Vector4f temp = new Vector4f(original.x,original.y,original.z,1f);
			Matrix4f.transform(transformation, temp, temp);
			newPoints[i] = new Vector3f(temp.x,temp.y,temp.z);
		}
		return new Triangle(newPoints[0],newPoints[1],newPoints[2]);
	}
	
}
