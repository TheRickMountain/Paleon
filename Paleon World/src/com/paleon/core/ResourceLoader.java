package com.paleon.core;

import com.paleon.graph.OBJLoader;
import com.paleon.textures.Texture;
import com.paleon.toolbox.MyFile;

public class ResourceLoader {

	public static void load() {
		ResourceManager.loadMesh("barrel", OBJLoader.loadMesh(new MyFile("model.obj")));
		ResourceManager.loadTexture("barrel", Texture.newTexture(new MyFile("diffuse.png"))
				.normalMipMap().create());
	}
	
}
