package com.paleon.core;

import java.util.HashMap;
import java.util.Map;

import com.paleon.textures.Texture;

public class ResourceManager {
	
	private static Map<String, Texture> textures = new HashMap<>();

	private ResourceManager() {}
	
	public static Texture loadTexture(String name, Texture texture){
		textures.put(name, texture);
		return texture;
    }
	
	public static Texture getTexture(String textureName) {
		Texture texture = textures.get(textureName);
		if(texture == null)
			System.err.println("There is no '" + textureName + "' texture!");
		return texture;
	}
	
}
