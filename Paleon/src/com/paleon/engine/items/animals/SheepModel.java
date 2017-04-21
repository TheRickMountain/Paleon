package com.paleon.engine.items.animals;

import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.items.GameObject;
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
		
		bodyMesh.setMaterial(material);
		frontLeftLegMesh.setMaterial(material);
		frontRightLegMesh.setMaterial(material);
		backLeftLegMesh.setMaterial(material);
		backRightLegMesh.setMaterial(material);
	}

	public static GameObject getBody() {
		GameObject gameObject = new GameObject();
		gameObject.addComponent(new MeshFilter(bodyMesh));
		gameObject.addComponent(new MeshRenderer());
		return gameObject;
	}

	public static GameObject getFrontLeftLeg() {
		GameObject gameObject = new GameObject();
		gameObject.addComponent(new MeshFilter(frontLeftLegMesh));
		gameObject.addComponent(new MeshRenderer());
		return gameObject;
	}

	public static GameObject getFrontRightLeg() {
		GameObject gameObject = new GameObject();
		gameObject.addComponent(new MeshFilter(frontRightLegMesh));
		gameObject.addComponent(new MeshRenderer());
		return gameObject;
	}

	public static GameObject getBackLeftLeg() {
		GameObject gameObject = new GameObject();
		gameObject.addComponent(new MeshFilter(backLeftLegMesh));
		gameObject.addComponent(new MeshRenderer());
		return gameObject;
	}

	public static GameObject getBackRightLeg() {
		GameObject gameObject = new GameObject();
		gameObject.addComponent(new MeshFilter(backRightLegMesh));
		gameObject.addComponent(new MeshRenderer());
		return gameObject;
	}
	
}
