package com.paleon.engine;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.loaders.OBJLoader;
import com.paleon.engine.loaders.TextureLoader;

public class ResourceManager {

	private static Map<String, ShaderProgram> shaders = new HashMap<String, ShaderProgram>();
	private static Map<String, Integer> textures = new HashMap<String, Integer>();
	private static Map<String, Mesh> meshes = new HashMap<String, Mesh>();
	
	public static ShaderProgram loadShader(String shaderName) {
		ShaderProgram shader = null;
		try {
			shader = new ShaderProgram();
			shader.createVertexShader("/shaders/" + shaderName + ".vs");
			shader.createFragmentShader("/shaders/" + shaderName + ".fs");
			shader.link();
			shaders.put(shaderName, shader);
			return shader;
		} catch (Exception e) {
			System.err.println("Failed to load " + shaderName + " shader");
			e.printStackTrace();
		}
		return null;
	}
	
	public static ShaderProgram getShader(String shaderName) {
		return shaders.get(shaderName);
	}
	
	public static int loadTexture(String path, String name) {
		int texture = 0;
		try {
			texture = TextureLoader.load(path);
			textures.put(name, texture);
		} catch (Exception e) {
			System.err.println("Failed to load " + path + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int getTexture(String name) {
		return textures.get(name);
	}
	
	public static int loadSkybox(String name) {
		int texture = 0;
		try {
			texture = TextureLoader.loadCubemap(name);
			textures.put(name, texture);
			return texture;
		} catch (Exception e) {
			System.err.println("Failed to load " + name + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int getSkybox(String name) {
		return textures.get(name);
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
		for(Entry<String, ShaderProgram> entry : shaders.entrySet()) {
			entry.getValue().cleanup();
		}
		for(Entry<String, Integer> entry : textures.entrySet()) {
			GL11.glDeleteTextures(entry.getValue());
		}
		for(Entry<String, Mesh> entry : meshes.entrySet()) {
			entry.getValue().cleanup();
		}
	}
}
