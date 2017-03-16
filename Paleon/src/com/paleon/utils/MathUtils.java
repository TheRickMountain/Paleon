package com.paleon.utils;

import java.util.List;

import com.paleon.core.Camera;
import com.paleon.core.Display;
import com.paleon.math.Matrix4f;
import com.paleon.math.Vector2f;
import com.paleon.math.Vector3f;

public class MathUtils {

	public static final float PI = 3.1415927f;
	public static final float RADIANS_TO_DEGREES = 180f / PI;
	public static final float DEGREES_TO_RADIANS = PI / 180;
	
	public static final Vector3f AXIS_X = new Vector3f(1, 0, 0);
    public static final Vector3f AXIS_Y = new Vector3f(0, 1, 0);
    public static final Vector3f AXIS_Z = new Vector3f(0, 0, 1);
    public static final Vector3f ZERO = new Vector3f(0, 0, 0);
    public static final Vector3f IDENTITY = new Vector3f(1, 1, 1);
	
	public static Matrix4f getOrthoProjectionMatrix(Matrix4f matrix, float left, float right, float bottom, float top) {
        matrix.m00 = 2.0f / (right - left);
        matrix.m11 = 2.0f / (top - bottom);
        matrix.m22 = -1.0f;
        matrix.m30 = -(right + left) / (right - left);
        matrix.m31 = -(top + bottom) / (top - bottom);
        return matrix;
	}
	
	public static Matrix4f getModelMatrix(Matrix4f matrix, float xPos, float yPos, float rotation, float xScale, float yScale){
		matrix.setIdentity();
		Matrix4f.translate(new Vector2f(xPos, yPos), matrix, matrix);
        Matrix4f.translate(new Vector2f(0.5f * xScale, 0.5f * yScale), matrix, matrix);
		Matrix4f.rotate(rotation * DEGREES_TO_RADIANS, AXIS_Z, matrix, matrix);
		Matrix4f.translate(new Vector2f(-0.5f * xScale, -0.5f * yScale), matrix, matrix);
		Matrix4f.scale(new Vector3f(xScale, yScale, 0.0f), matrix, matrix); 
		return matrix;
	}
	
	public static Matrix4f getViewMatrix(Matrix4f matrix, Camera camera) {
		matrix.setIdentity();
		Matrix4f.translate(new Vector3f(
				-(camera.getX() - (Display.getWidth() * camera.getZoom()) / 2), 
				-(camera.getY() - (Display.getHeight() * camera.getZoom()) / 2), 0), matrix, matrix);
		return matrix;
	}
	
	public static float getDistance(float x1, float y1, float x2, float y2) {
		float dX = x1 - x2;
		float dY = y1 - y2;
		return (float) Math.sqrt((dX * dX) + (dY * dY));
	}
	
	public static float getLerp(float p1, float p2, float alpha) {
		return p1 + alpha * (p2 - p1);
	}
	
	public static float getDirection(float x1, float y1, float x2, float y2) {
		float angle = (float) Math.toDegrees((Math.atan2(y2 - y1, x2 - x1)));
		if(angle < 0) {
			angle += 360;
		}
		return angle;
	}
	
	public static float getLengthdirX(float length, float direction) {
		return (float) (Math.cos(direction * (Math.PI / 180)) * length);
	}
	
	public static float getLengthdirY(float length, float direction) {
		return (float) (Math.sin(direction * (Math.PI / 180)) * length);
	}
	
	public static float[] listToArray(List<Float> list) {
		 int size = list != null ? list.size() : 0;
		 float[] floatArr = new float[size];
		 for (int i = 0; i < size; i++) {
			 floatArr[i] = list.get(i);
		 }
		 return floatArr;
	}
	
	public static int compareTo(int a, int b) {
		if(a > b) {
			return 1;
		} else if(a == b) {
			return 0;
		} else {
			return -1;
		}
	}
	
}
