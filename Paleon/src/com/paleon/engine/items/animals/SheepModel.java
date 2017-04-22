package com.paleon.engine.items.animals;

import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.items.Entity;
import com.paleon.engine.loaders.OBJLoader;
import com.paleon.engine.loaders.TextureLoader;

public class SheepModel {

	private static Material material;
	private static Mesh bodyMesh, frontLeftLegMesh, frontRightLegMesh, backLeftLegMesh, backRightLegMesh;
	
	public static void init() {
		material = new Material(TextureLoader.load("/prefab/sheep.png"));
		
		bodyMesh = OBJLoader.loadMesh("/prefab/chest.obj");
		frontLeftLegMesh = OBJLoader.loadMesh("/prefab/leftFrontLeg.obj");
		frontRightLegMesh = OBJLoader.loadMesh("/prefab/rightFrontLeg.obj");
		backLeftLegMesh = OBJLoader.loadMesh("/prefab/leftFrontLeg.obj");
		backRightLegMesh = OBJLoader.loadMesh("/prefab/rightFrontLeg.obj");
	}

	public static Entity getBody() {
		Entity gameObject = new Entity(bodyMesh, material);
		return gameObject;
	}

	public static Entity getFrontLeftLeg() {
		Entity gameObject = new Entity(frontLeftLegMesh, material);
		return gameObject;
	}

	public static Entity getFrontRightLeg() {
		Entity gameObject = new Entity(frontRightLegMesh, material);
		return gameObject;
	}

	public static Entity getBackLeftLeg() {
		Entity gameObject = new Entity(backLeftLegMesh, material);
		return gameObject;
	}

	public static Entity getBackRightLeg() {
		Entity gameObject = new Entity(backRightLegMesh, material);
		return gameObject;
	}
	
}
