package com.paleon.engine.processing;

import com.paleon.engine.Display;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Entity;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.Plane;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class Frustum {

	private static final float HALF_DIAGONAL = 1.7072f / 2f;
	
	private Camera camera;
	private Plane[] frustum;
	private float farDistance;
	private float heightFar;
	private float widthFar;
	
	private float cameraX;
	private float cameraZ;

	public Frustum(Camera camera) {
		frustum = new Plane[2];
		farDistance = camera.farPlane;
		heightFar = (float) (2 * Math.tan((camera.fieldOfView / 2) * MathUtils.DEGREES_TO_RADIANS) * farDistance);
		widthFar = heightFar * (Display.getWidth() / Display.getHeight());
		this.camera = camera;
	}
	
	public void updatePlanes() {
		Vector3f point = new Vector3f(camera.getX(), camera.getY(), camera.getZ());
		Vector3f direction = new Vector3f();
		Vector3f up = new Vector3f();
		Vector3f down = new Vector3f();
		Vector3f left = new Vector3f();
		Vector3f right = new Vector3f();
		calculateVectorDirections(direction, up, down, right, left);

		Vector3f farPlaneCenter = Vector3f.add(point, new Vector3f(direction.x * farDistance,
				direction.y * farDistance, direction.z * farDistance), null);
		frustum[0] = new Plane(
				calculatePlaneNormal(point, farPlaneCenter, right, up, widthFar / 2), point);
		frustum[1] = new Plane(calculatePlaneNormal(point, farPlaneCenter, right, down,
				-widthFar / 2), point);
		
		cameraX = camera.getPosition().x;
		cameraZ = camera.getPosition().z;
	}
	
	public boolean testEntityInView(Entity entity) {
		boolean render = true;
		float entityX = entity.position.x;
		float entityZ = entity.position.z;
		if(MathUtils.getDistanceBetweenPoints(entityX, entityZ, cameraX, cameraZ) >= 350) {
			render = false;
		} else {
			Vector3f point = new Vector3f(entityX, entity.position
					.getY(), entityZ);
			for (Plane plane : frustum) {
				float distance = plane.getSignedDistance(point);
				if (distance < -entity.getFurthestPoint()) {
					render = false;
				}
			}
		}
		return render;
	}
	
	public boolean testTerrainInView(TerrainBlock terrain) {
		boolean render = true;
		float terrainX = terrain.getX();
		float terrainZ = terrain.getZ();
		if(MathUtils.getDistanceBetweenPoints(terrainX, terrainZ, 
				cameraX, cameraZ) >= 350) {
			render = false;
		} else {
			Vector3f point = new Vector3f(terrainX, 0, terrainZ);
			for (Plane plane : frustum) {
				float distance = plane.getSignedDistance(point);
				if (distance < -terrain.getSize() * HALF_DIAGONAL) {
					render = false;
				}
			}
		}
		return render;
	}
	
	public boolean testWaterTileInView(WaterTile tile) {
		boolean render = true;
		float waterX = tile.getX();
		float waterZ = tile.getZ();
		if(MathUtils.getDistanceBetweenPoints(waterX, waterZ, 
				cameraX, cameraZ) >= 350) {
			render = false;
		} else {
			Vector3f point = new Vector3f(waterX, tile.getHeight(), waterZ);
			for(Plane plane : frustum) {
				float distance = plane.getSignedDistance(point);
				if(distance < -(WaterTile.TILE_SIZE * 1.6f) * HALF_DIAGONAL) {
					render = false;
				}
			}
		}	
		return render;
	}
	
	private Vector3f calculatePlaneNormal(Vector3f point, Vector3f farPlaneCenter, Vector3f right,
			Vector3f onPlane, float side) {
		Vector3f sidePoint = Vector3f.add(farPlaneCenter, new Vector3f(right.x * side, right.y
				* side, right.z * side), null);
		Vector3f alongPlane = Vector3f.sub(sidePoint, point, null);
		alongPlane.normalise();
		return Vector3f.cross(onPlane, alongPlane, null);
	}

	private void calculateVectorDirections(Vector3f direction, Vector3f up, Vector3f down,
			Vector3f right, Vector3f left) {
		Vector4f originalDirection = new Vector4f(0, 0, -1, 1);
		Vector4f originalUp = new Vector4f(0, 1, 0, 1);
		Matrix4f rotation = new Matrix4f();
		rotation.rotate(-camera.getYaw() * MathUtils.DEGREES_TO_RADIANS, new Vector3f(0, 1, 0));
		rotation.rotate(-camera.getPitch() * MathUtils.DEGREES_TO_RADIANS, new Vector3f(1, 0, 0));
		Matrix4f.transform(rotation, originalUp, originalUp);
		Matrix4f.transform(rotation, originalDirection, originalDirection);
		up.set(originalUp);
		direction.set(originalDirection);
		right.set(Vector3f.cross(direction, up, null));
		right.normalise();
		right.negate(left);
		up.negate(down);
	}
	
}
