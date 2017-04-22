package com.paleon.engine;

public class Resources {

	public static void load() {
		
		/*** Textures ***/
		// Terrain
		ResourceManager.loadTexture("/biomes/blendmap.png", "blendMap");
		ResourceManager.loadTexture("/textures/sand.png", "sand");
		ResourceManager.loadTexture("/textures/dirt.png", "dirt");
		ResourceManager.loadTexture("/textures/dry_grass.png", "grass");
		
		// Water
		ResourceManager.loadTexture("/water/dudvMap.png", "dudvMap");
		ResourceManager.loadTexture("/water/normalMap.png", "normalMap");
		
		// Skybox
		ResourceManager.loadSkybox("sunset");
		ResourceManager.loadSkybox("sunny2");
		ResourceManager.loadSkybox("night");
		
		/*** Fern ***/
		ResourceManager.loadTexture("/prefabs/fern/fern.png", "fern");
		ResourceManager.loadMesh("/prefabs/fern/fern.obj", "fern");
		/*** *** ***/
		
		/*** Birch ***/
		ResourceManager.loadTexture("/prefabs/tree/pine/bark.png", "bark");
		ResourceManager.loadMesh("/prefabs/tree/pine/bark.obj", "bark");
		
		ResourceManager.loadTexture("/prefabs/tree/pine/leaves.png", "leaves");
		ResourceManager.loadMesh("/prefabs/tree/pine/leaves.obj", "leaves");
		/*** *** ***/
		
		/*** Shroom ***/
		ResourceManager.loadTexture("/prefabs/shroom/shroom.png", "shroom");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_1.obj", "shroom_1");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_2.obj", "shroom_2");
		ResourceManager.loadMesh("/prefabs/shroom/shroom_3.obj", "shroom_3");
		/*** *** ***/
		
		/*** Flint ***/
		ResourceManager.loadTexture("/prefabs/flint/flint.png", "flint");
		ResourceManager.loadMesh("/prefabs/flint/flint.obj", "flint");
		/*** *** ***/
		
		/*** Stick ***/
		ResourceManager.loadTexture("/prefabs/stick/stick.png", "stick");
		ResourceManager.loadMesh("/prefabs/stick/stick.obj", "stick");
		/*** *** ***/
		
		/*** Wheat ***/
		ResourceManager.loadTexture("/prefabs/wheat/wheat.png", "wheat");
		ResourceManager.loadMesh("/prefabs/wheat/wheat.obj", "wheat");
		/*** *** ***/
		
		/*** Player ***/
		ResourceManager.loadTexture("/prefabs/human/skin.png", "skin");
		ResourceManager.loadTexture("/prefabs/human/eyes.png", "eyes");
		
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
	}
	
}
