package com.paleon.ecs;

import com.paleon.graph.Material;
import com.paleon.graph.Mesh;

public class Entity {

	private Transform transform;
	
	private Mesh mesh;
	private Material material;
	
	public Entity(Mesh mesh, Material material) {
		this.mesh = mesh;
		this.material = material;
		this.transform = new Transform();
	}
	
	public void init() {
		
	}
	
	public void update(float dt) {
		
	}
	
	public Transform getTransform() {
		return transform;
	}
	
	public Mesh getMesh() {
		return mesh;
	}
	
	public Material getMaterial() {
		return material;
	}
	
}
