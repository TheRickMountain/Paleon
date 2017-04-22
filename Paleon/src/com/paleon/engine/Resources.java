package com.paleon.engine;

import com.paleon.engine.toolbox.MyFile;
import com.paleon.textures.Texture;

public class Resources {

	public static void load() {
		
		/*** Textures ***/
		// Terrain
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("biomes/blendmap.png"))
				.normalMipMap().create(), "blendMap");
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("textures/sand.png"))
				.normalMipMap().create(), "sand");
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("textures/dirt.png"))
				.normalMipMap().create(), "dirt");
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("textures/dry_grass.png"))
				.normalMipMap().create(), "grass");
		
		// Water
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("water/dudvMap.png"))
				.anisotropic().create(), "dudvMap");
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("water/normalMap.png"))
				.anisotropic().create(), "normalMap");
		
		// Skybox
		ResourceManager.loadSkybox("sunset");
		ResourceManager.loadSkybox("sunny2");
		ResourceManager.loadSkybox("night");
		
		/*** Fern ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/fern/fern.png"))
				.normalMipMap().create(), "fern");
		ResourceManager.loadMesh("/prefabs/fern/fern.obj", "fern");
		/*** *** ***/
		
		/*** Birch ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/tree/pine/bark.png"))
				.normalMipMap().create(), "bark");
		ResourceManager.loadMesh("/prefabs/tree/pine/bark.obj", "bark");
		
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/tree/pine/leaves.png"))
				.normalMipMap().create(), "leaves");
		ResourceManager.loadMesh("/prefabs/tree/pine/leaves.obj", "leaves");
		/*** *** ***/
		
		/*** Shroom ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/shroom/shroom.png"))
				.normalMipMap().create(), "shroom");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_1.obj", "shroom_1");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_2.obj", "shroom_2");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_3.obj", "shroom_3");
		/*** *** ***/
		
		/*** Flint ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/flint/flint.png"))
				.normalMipMap().create(), "flint");
		ResourceManager.loadMesh("/prefabs/flint/flint.obj", "flint");
		/*** *** ***/
		
		/*** Stick ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/stick/stick.png"))
				.normalMipMap().create(), "stick");
		ResourceManager.loadMesh("/prefabs/stick/stick.obj", "stick");
		/*** *** ***/
		
		/*** Wheat ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/wheat/wheat.png"))
				.normalMipMap().create(), "wheat");
		ResourceManager.loadMesh("/prefabs/wheat/wheat.obj", "wheat");
		/*** *** ***/
		
		/*** Player ***/
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/human/skin.png"))
				.normalMipMap().create(), "skin");
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefabs/human/eyes.png"))
				.normalMipMap().create(), "eyes");
		
		ResourceManager.loadMesh("/prefabs/human/eyes.obj", "eyes");
		
		ResourceManager.loadMesh("/prefabs/human/rightArm.obj", "leftArm");
		ResourceManager.loadMesh("/prefabs/human/leftArm.obj", "rightArm");
		ResourceManager.loadMesh("/prefabs/human/leftForearm.obj", "rightForearm");
		ResourceManager.loadMesh("/prefabs/human/rightForearm.obj", "leftForearm");
		ResourceManager.loadMesh("/prefabs/human/body.obj", "body");
		ResourceManager.loadMesh("/prefabs/human/head.obj", "head");
		ResourceManager.loadMesh("/prefabs/human/hip.obj", "hip");
		ResourceManager.loadMesh("/prefabs/human/shin.obj", "shin");
		/*** *** ***/
		
		ResourceManager.loadTexture(Texture.newTexture(new MyFile("prefab/sheep.png"))
				.normalMipMap().create(), "sheep");
	}
	
}
