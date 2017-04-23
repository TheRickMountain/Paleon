package com.paleon.engine;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.graph.Mesh;
import com.paleon.engine.loaders.OBJLoader;
import com.paleon.engine.loaders.TextureLoader;
import com.paleon.textures.Texture;

public class ResourceManager {

	private static Map<String, Texture> textures = new HashMap<String, Texture>();
	private static Map<String, Integer> skyboxes = new HashMap<String, Integer>();
	private static Map<String, Mesh> meshes = new HashMap<String, Mesh>();
	
	public static void loadTexture(Texture texture, String name) {
		textures.put(name, texture);
	}
	
	public static Texture getTexture(String name) {
		return textures.get(name);
	}
	
	public static int loadSkybox(String name) {
		int texture = 0;
		try {
			texture = TextureLoader.loadCubemap(name);
			skyboxes.put(name, texture);
			return texture;
		} catch (Exception e) {
			System.err.println("Failed to load " + name + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int getSkybox(String name) {
		return skyboxes.get(name);
	}
	
	public static Mesh loadMesh(String path, String name) {
		try {
			Mesh mesh = null;
			mesh = OBJLoader.loadMesh(path);
			meshes.put(name, mesh);
			return mesh;
		} catch (Exception e) {
			System.err.println("Failed to load " + path + " mesh");
			e.printStackTrace();
		}
		return null;
	}
	
	public static Mesh getMesh(String name) {
		return meshes.get(name);
	}
	
	public static void clear() {
		for(Entry<String, Texture> entry : textures.entrySet()) {
			entry.getValue().cleanup();
		}
		for(Entry<String, Integer> entry : skyboxes.entrySet()) {
			GL11.glDeleteTextures(entry.getValue());
		}
		for(Entry<String, Mesh> entry : meshes.entrySet()) {
			entry.getValue().cleanup();
		}
	}
}
