package com.paleon.engine.toolbox;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.FloatBuffer;
import java.nio.IntBuffer;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

import org.lwjgl.BufferUtils;

import com.paleon.engine.items.Camera;
import com.paleon.maths.altvecmath.Matrix4;
import com.paleon.maths.altvecmath.Vector3;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector3f;

public class MathUtils {
	
	public static double NANOSECOND = 1000000000;
	private static List<Integer> idCache = new ArrayList<Integer>();
	public static Random rand = new Random();
	public static final float PI = 3.1415927f;
	public static final float RADIANS_TO_DEGREES = 180f / PI;
	public static final float DEGREES_TO_RADIANS = PI / 180;
	
	public static final Vector3f X_AXIS = new Vector3f(1, 0, 0);
    public static final Vector3f Y_AXIS = new Vector3f(0, 1, 0);
    public static final Vector3f Z_AXIS = new Vector3f(0, 0, 1);
	
	@SuppressWarnings("resource")
	public static String loadResource(String fileName) throws Exception {
        String result = "";
        try (InputStream in = MathUtils.class.getClass().getResourceAsStream(fileName)) {
            result = new Scanner(in, "UTF-8").useDelimiter("\\A").next();
        }
        return result;
    }
	
	public static float getAverageOfList(List<Float> numbers) {
		float total = 0;
		for (Float number : numbers) {
			total += number;
		}
		return total / numbers.size();
	}
	
	public static List<String> readAllLines(String fileName) throws Exception {
        List<String> list = new ArrayList<>();
        try (BufferedReader br = new BufferedReader(new InputStreamReader(MathUtils.class.getClass().getResourceAsStream(fileName)))) {
            String line;
            while ((line = br.readLine()) != null) {
                list.add(line);
            }
        }
        return list;
    }
	
	public static Matrix4f getModelMatrix(float xPos, float yPos, float rotation, float xScale, float yScale){
		Matrix4f matrix = new Matrix4f();
        Matrix4f.translate(new Vector2f(xPos, yPos), matrix, matrix);
        Matrix4f.translate(new Vector2f(0.5f * xScale, 0.5f * yScale), matrix, matrix);
		Matrix4f.rotate(rotation* DEGREES_TO_RADIANS, Z_AXIS, matrix, matrix);
		Matrix4f.translate(new Vector2f(-0.5f * xScale, -0.5f * yScale), matrix, matrix);
		Matrix4f.scale(new Vector3f(xScale, yScale, 0.0f), matrix, matrix);
        
		return matrix;
	}
	
	public static Matrix4f getEulerModelMatrix(Vector3f position, Vector3f rotation, Vector3f scale) {
		com.paleon.maths.altvecmath.Transform transform = new com.paleon.maths.altvecmath.Transform();
		transform.reset()
        .rotateSelf(Vector3.AXIS_X, rotation.x * DEGREES_TO_RADIANS)
        .rotateSelf(Vector3.AXIS_Z, rotation.z * DEGREES_TO_RADIANS)
        .rotateSelf(Vector3.AXIS_Y, rotation.y * DEGREES_TO_RADIANS)
        .scaleSelf(new Vector3(scale.x, scale.y, scale.z))
        .translateSelf(new Vector3(position.x, position.y, position.z));
		Matrix4 tempMatrix = transform.getMatrix();
		Matrix4f matrix = new Matrix4f();
		matrix.m00 = tempMatrix.get(0, 0);
		matrix.m01 = tempMatrix.get(0, 1);
		matrix.m02 = tempMatrix.get(0, 2);
		matrix.m03 = tempMatrix.get(0, 3);
		matrix.m10 = tempMatrix.get(1, 0);
		matrix.m11 = tempMatrix.get(1, 1);
		matrix.m12 = tempMatrix.get(1, 2);
		matrix.m13 = tempMatrix.get(1, 3);
		matrix.m20 = tempMatrix.get(2, 0);
		matrix.m21 = tempMatrix.get(2, 1);
		matrix.m22 = tempMatrix.get(2, 2);
		matrix.m23 = tempMatrix.get(2, 3);
		matrix.m30 = tempMatrix.get(3, 0);
		matrix.m31 = tempMatrix.get(3, 1);
		matrix.m32 = tempMatrix.get(3, 2);
		matrix.m33 = tempMatrix.get(3, 3);
		return matrix;
    }
	
	public static Matrix4f getEulerModelMatrix(Matrix4f matrix, Vector3f position, Vector3f rotation, Vector3f scale) {
		com.paleon.maths.altvecmath.Transform transform = new com.paleon.maths.altvecmath.Transform();
		transform.reset()
        .rotateSelf(Vector3.AXIS_X, rotation.x * DEGREES_TO_RADIANS)
        .rotateSelf(Vector3.AXIS_Z, rotation.z * DEGREES_TO_RADIANS)
        .rotateSelf(Vector3.AXIS_Y, rotation.y * DEGREES_TO_RADIANS)
        .scaleSelf(new Vector3(scale.x, scale.y, scale.z))
        .translateSelf(new Vector3(position.x, position.y, position.z));
		Matrix4 tempMatrix = transform.getMatrix();
		matrix.m00 = tempMatrix.get(0, 0);
		matrix.m01 = tempMatrix.get(0, 1);
		matrix.m02 = tempMatrix.get(0, 2);
		matrix.m03 = tempMatrix.get(0, 3);
		matrix.m10 = tempMatrix.get(1, 0);
		matrix.m11 = tempMatrix.get(1, 1);
		matrix.m12 = tempMatrix.get(1, 2);
		matrix.m13 = tempMatrix.get(1, 3);
		matrix.m20 = tempMatrix.get(2, 0);
		matrix.m21 = tempMatrix.get(2, 1);
		matrix.m22 = tempMatrix.get(2, 2);
		matrix.m23 = tempMatrix.get(2, 3);
		matrix.m30 = tempMatrix.get(3, 0);
		matrix.m31 = tempMatrix.get(3, 1);
		matrix.m32 = tempMatrix.get(3, 2);
		matrix.m33 = tempMatrix.get(3, 3);
		return matrix;
    }
	
	
	public static Matrix4f getModelMatrix(Vector3f offset, float rotX, float rotY, float rotZ, float scale){
		Matrix4f matrix = new Matrix4f();
		Matrix4f.translate(offset, matrix, matrix);
		Matrix4f.rotate(rotX * DEGREES_TO_RADIANS, new Vector3f(1, 0, 0), matrix, matrix);
		Matrix4f.rotate(rotY * DEGREES_TO_RADIANS, new Vector3f(0, 1, 0), matrix, matrix);
		Matrix4f.rotate(rotZ * DEGREES_TO_RADIANS, new Vector3f(0, 0, 1), matrix, matrix);
		Matrix4f.scale(new Vector3f(scale, scale, scale), matrix, matrix);
		return matrix;
	}
	
	public static Matrix4f getViewMatrix(Camera camera){
		Matrix4f matrix = new Matrix4f();
		matrix.setIdentity();
		Matrix4f.rotate((float) Math.toRadians(camera.getPitch()), new Vector3f(1, 0, 0), matrix, matrix);
		Matrix4f.rotate((float) Math.toRadians(camera.getYaw()), new Vector3f(0, 1, 0), matrix, matrix);
		Vector3f cameraPos = camera.getPosition();
		Vector3f negativeCameraPos = new Vector3f(-cameraPos.x, -cameraPos.y, -cameraPos.z);
		Matrix4f.translate(negativeCameraPos, matrix, matrix);
		return matrix;
	}
	
	public static Matrix4f getPerspectiveProjectionMatrix(Matrix4f matrix, float fov, float width, float height, float zNear, float zFar){
		float aspectRatio = (float) width/ (float)height;
		float y_scale = (float) ((1f / Math.tan((fov / 2) * DEGREES_TO_RADIANS)) * aspectRatio);
		float x_scale = y_scale / aspectRatio;
		float frustum_length = zFar - zNear;
		
		matrix.m00 = x_scale;
		matrix.m11 = y_scale;
		matrix.m22 = -((zFar + zNear) / frustum_length);
		matrix.m23 = -1;
		matrix.m32 = -((2 * zFar * zNear) / frustum_length);
		matrix.m33 = 0;
		
		return matrix; 
	}
	
	public static Matrix4f getOrtho2DProjectionMatrix(Matrix4f matrix, float left, float right, float bottom, float top) {
        matrix.m00 = 2.0f / (right - left);
        matrix.m11 = 2.0f / (top - bottom);
        matrix.m22 = -1.0f;
        matrix.m30 = -(right + left) / (right - left);
        matrix.m31 = -(top + bottom) / (top - bottom);
        return matrix;
	}
	
	public static Vector2f getRotationAroundPoint(Vector3f center, Vector3f point, float angle){
		Vector2f temp = new Vector2f();
		temp.x =  (float) (Math.cos(angle) * (point.x - center.x) - Math.sin(angle) * (point.z - center.z) + center.x);
		temp.y =  (float) (Math.sin(angle) * (point.x - center.x) + Math.cos(angle) * (point.z - center.z) + center.z);
		return temp;
	}
	
	public static float getAngle(Vector3f target, Vector3f current) {
		float angle = (float)Math.toDegrees(Math.atan2(target.z - current.z, target.x - current.x));
		
		if(angle < 0) {
			angle += 360;
		}
		
		return angle - 90;
	}
	
	public static float getRotationBetweenPoints(Vector3f player, Vector3f target){
		return (float) ((Math.atan2(target.z - player.z, target.x - player.x)) * RADIANS_TO_DEGREES);
	}
	
	public static float getRotationBetweenPoints(Vector3f player, Vector2f target){
		return (float) ((Math.atan2(target.y - player.z, target.x - player.x)) * RADIANS_TO_DEGREES);
	}
	
	public static float getRotationBetweenPoints(float x1, float z1, float x2, float z2){
		return (float) ((Math.atan2(z2 - z1, x2 - x1)) * RADIANS_TO_DEGREES);
	}
	
	public static float getDistanceBetweenPoints(float x1, float y1, float x2, float y2) {
		float dX = x1 - x2;
		float dY = y1 - y2;
		float distance = (float) Math.sqrt((dX * dX) + (dY * dY));
		return distance;
	}
	
	public static float getDistanceBetweenPoints(Vector2f pos1, Vector3f pos2) {
		float dX = pos1.x - pos2.x;
		float dY = pos1.y - pos2.z;
		float distance = (float) Math.sqrt((dX * dX) + (dY * dY));
		return distance;
	}

	public static float getDistanceBetweenPoints(float x1, float y1, float z1, float x2, float y2, float z2) {
		float dX = x1 - x2;
		float dY = y1 - y2;
		float dZ = z1 - z2;
		float distance = (float) Math.sqrt((dX * dX) + (dY * dY) + (dZ * dZ));
		return distance;
	}
	
	public static float getDistanceBetweenPoints(Vector3f vec1, Vector3f vec2) {
		float dX = vec1.x - vec2.x;
		float dY = vec1.y - vec2.y;
		float dZ = vec1.z - vec2.z;
		float distance = (float) Math.sqrt((dX * dX) + (dY * dY) + (dZ * dZ));
		return distance;
	}
	
	public static IntBuffer dataToIntBuffer(int[] data){
		IntBuffer buffer = BufferUtils.createIntBuffer(data.length);
		buffer.put(data).flip();
		return buffer;
	}
	
	public static FloatBuffer dataToFloatBuffer(float[] data){
		FloatBuffer buffer = BufferUtils.createFloatBuffer(data.length);
		buffer.put(data).flip();
		return buffer;
	}
	
	public static int[] listIntToArray(List<Integer> list) {
        int[] result = list.stream().mapToInt((Integer v) -> v).toArray();
        return result;
    }
	
	public static float[] listFloatToArray(List<Float> list) {
        int size = list != null ? list.size() : 0;
        float[] floatArr = new float[size];
        for (int i = 0; i < size; i++) {
            floatArr[i] = list.get(i);
        }
        return floatArr;
    }
	
	public static int generateId(){
		int id = getIdByColor(generateColor());
		for(int temp : idCache){
			if(id == temp){
				generateId();
			}
		}
		return id;
	}
	
	public static Color generateColor(){
		int r = rand.nextInt(256);
		int g = rand.nextInt(256);
		int b = rand.nextInt(256);
		return new Color(r, g, b);
	}
	
	public static int getIdByColor(Color color){
		return (int)color.getR() + (int)color.getG() * 256 + (int)color.getB() * 256 * 256;
	}
	
	public static int getIdByColor(int r, int g, int b){
		return (r)|(g<<8)|(b<<16);
	}
	
	public static Color getColorById(int id){
		int r = (id & 0x000000FF) >> 0;
		int g = (id & 0x0000FF00) >> 8;
		int b = (id & 0x00FF0000) >> 16;
		return new Color(r, g, b);
	}
	
	public static Vector3f getRandomPointInCirle(int x1, int z1, int maxRadius){
		int angle = rand.nextInt(360);
		int radius = rand.nextInt(maxRadius);
		int x2 = (int) Math.round(x1 + radius * Math.cos(angle));
		int z2 = (int) Math.round(z1 + radius * Math.sin(angle));
		return new Vector3f(x2, 0, z2);
	}
	
	public static Vector3f calculateNormal(Vector3f point0, Vector3f point1, Vector3f point2) {
		Vector3f vectorA = Vector3f.sub(point1, point0, null);
		Vector3f vectorB = Vector3f.sub(point2, point0, null);	
		Vector3f normal = Vector3f.cross(vectorA, vectorB, null);
		normal.normalise();
		return normal;
	}
	
	public static float barryCentric(Vector3f p1, Vector3f p2, Vector3f p3, Vector2f pos) {
		float det = (p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z);
		float l1 = ((p2.z - p3.z) * (pos.x - p3.x) + (p3.x - p2.x) * (pos.y - p3.z)) / det;
		float l2 = ((p3.z - p1.z) * (pos.x - p3.x) + (p1.x - p3.x) * (pos.y - p3.z)) / det;
		float l3 = 1.0f - l1 - l2;
		return l1 * p1.y + l2 * p2.y + l3 * p3.y;
	}
	
	public static float[] joinArrays(float[] vertices, float[] uvs) {
		float[] data = new float[vertices.length + uvs.length];
		int count = 0;
		int vertexCount = 0;
		int uvsCount = 0;
		for(int i = 0; i < data.length / 4; i++) {
			data[count] = vertices[vertexCount];
			vertexCount++;
			count++;
			data[count] = vertices[vertexCount];
			vertexCount++;
			count++;
			data[count] = uvs[uvsCount];
			uvsCount++;
			count++;
			data[count] = uvs[uvsCount];
			uvsCount++;
			count++;
		}
		return data;
	}
	
	public static String [] removeEmptyStrings(String[] data) {
        ArrayList<String> result = new ArrayList<String>();

        for (int i = 0; i < data.length; i++)
            if(!data[i].equals(""))
                result.add(data[i]);

        String[] res = new String[result.size()];
        result.toArray(res);

        return res;
    }
	
}
