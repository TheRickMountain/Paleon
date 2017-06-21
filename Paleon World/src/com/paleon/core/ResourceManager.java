package com.paleon.core;

import java.util.HashMap;
import java.util.Map;

import com.paleon.graph.Mesh;
import com.paleon.textures.Texture;


public class ResourceManager {
	
	private static Map<String, Texture> textures = new HashMap<>();
	private static Map<String, Mesh> meshes = new HashMap<>();

	public static void loadMesh(String name, Mesh mesh) {
        meshes.put(name, mesh);
    }

    public static Mesh getMesh(String name) {
    	Mesh mesh = meshes.get(name);
    	if(mesh == null)
			System.err.println("There is no '" + name + "' texture!");
        return meshes.get(name);
    }
	
	public static void loadTexture(String name, Texture texture){
		textures.put(name, texture);
    }
	
	public static Texture getTexture(String name) {
		Texture texture = textures.get(name);
		if(texture == null)
			System.err.println("There is no '" + name + "' texture!");
		return texture;
	}
	
	public static void cleanup() {
		for(Texture texture : textures.values()) {
			texture.cleanup();
		}
		
		for(Mesh mesh : meshes.values()) {
			mesh.cleanup();
		}
	}
	
}
