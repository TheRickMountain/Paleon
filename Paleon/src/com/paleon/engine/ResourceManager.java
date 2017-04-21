package com.paleon.engine;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.graph.Material;
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
	
	public static int loadTexture(String texturePath, String textureName) {
		int texture = 0;
		try {
			texture = TextureLoader.load(texturePath);
			textures.put(textureName, texture);
		} catch (Exception e) {
			System.err.println("Failed to load " + texturePath + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int loadSTBImage(String texturePath, String textureName) {
		int texture = 0;
		try {
			texture = TextureLoader.load(texturePath);
			textures.put(textureName, texture);
		} catch (Exception e) {
			System.err.println("Failed to load " + texturePath + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int getTexture(String textureName) {
		return textures.get(textureName);
	}
	
	public static int loadSkybox(String skyboxName) {
		int texture = 0;
		try {
			texture = TextureLoader.loadCubemap(skyboxName);
			textures.put(skyboxName, texture);
			return texture;
		} catch (Exception e) {
			System.err.println("Failed to load " + skyboxName + " texture");
			e.printStackTrace();
		}
		return texture;
	}
	
	public static int getSkybox(String skyboxName) {
		return textures.get(skyboxName);
	}
	
	public static Mesh loadMesh(String meshPath, String meshName, Material material) {
		try {
			Mesh mesh = null;
			mesh = OBJLoader.loadMesh(meshPath);
			mesh.setMaterial(material);
			meshes.put(meshName, mesh);
			return mesh;
		} catch (Exception e) {
			System.err.println("Failed to load " + meshPath + " mesh");
			e.printStackTrace();
		}
		return null;
	}
	
	public static Mesh getMesh(String meshName) {
		return meshes.get(meshName);
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
