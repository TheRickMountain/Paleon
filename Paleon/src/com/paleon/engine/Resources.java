package com.paleon.engine;

import com.paleon.engine.graph.Material;
import com.paleon.engine.loaders.TextureLoader;

public class Resources {

	public static void load() {
		
		/*** Textures ***/
		ResourceManager.loadTexture("/biomes/blendmap.png", "blendMap");
		ResourceManager.loadTexture("/textures/sand.png", "sand");
		ResourceManager.loadTexture("/textures/preview.png", "dirt");
		ResourceManager.loadTexture("/textures/grass.png", "grass");
		
		ResourceManager.loadTexture("/prefabs/human/skin.png", "skin");
		ResourceManager.loadTexture("/prefabs/human/eyes.png", "eyes");
		
		// Water
		ResourceManager.loadTexture("/water/dudvMap.png", "dudvMap");
		ResourceManager.loadTexture("/water/normalMap.png", "normalMap");
		
		// Skybox
		ResourceManager.loadSkybox("sunset");
		ResourceManager.loadSkybox("sunny2");
		ResourceManager.loadSkybox("night");
		
		// GUI
		ResourceManager.loadSTBImage("/gui/cross.png", "ui_cross");
		ResourceManager.loadSTBImage("/gui/elements/button.png", "ui_button");
		ResourceManager.loadSTBImage("/gui/slot.png", "ui_slot");
		ResourceManager.loadSTBImage("/gui/icons/flint.png", "ui_flint");
		ResourceManager.loadSTBImage("/gui/icons/stick.png", "ui_stick");
		ResourceManager.loadSTBImage("/gui/icons/shroom.png", "ui_shroom");
		ResourceManager.loadSTBImage("/gui/icons/wheat.png", "ui_wheat");
		ResourceManager.loadSTBImage("/gui/icons/seeds of wheat.png", "ui_seeds_of_wheat");
		ResourceManager.loadSTBImage("/gui/icons/rope.png", "ui_rope");
		ResourceManager.loadSTBImage("/gui/icons/stone axe.png", "ui_stone_axe");
		ResourceManager.loadSTBImage("/gui/icons/apple.png", "ui_apple");
		ResourceManager.loadSTBImage("/gui/icons/flour.png", "ui_flour");
		ResourceManager.loadSTBImage("/gui/icons/bread.png", "ui_bread");
		ResourceManager.loadSTBImage("/gui/icons/dough.png", "ui_dough");
		ResourceManager.loadSTBImage("/gui/icons/clay cup of water.png", "ui_clay_cup_of_water");
		ResourceManager.loadSTBImage("/gui/icons/clay cup.png", "ui_clay_cup");
		
		ResourceManager.loadSTBImage("/gui/bar/bar_back.png", "bar_back");
		ResourceManager.loadSTBImage("/gui/bar/bar_front.png", "bar_front");
		ResourceManager.loadSTBImage("/gui/bar/health.png", "health");
		ResourceManager.loadSTBImage("/gui/bar/hunger.png", "hunger");
		ResourceManager.loadSTBImage("/gui/bar/thirst.png", "thirst");
		
		// Font
		ResourceManager.loadSTBImage("/fonts/primitive_font.png", "primitive_font");
		/*** *** ***/
		
		/*** Bush ***/
		Material bushMat = new Material(TextureLoader.load("/prefabs/bush/bush.png"));
		bushMat.setHasTransparency(true);
		ResourceManager.loadMesh("/prefabs/bush/bush.obj", "bush", bushMat);
		/*** *** ***/
		
		/*** Birch ***/
		Material barkMat = new Material(TextureLoader.load("/prefabs/tree/pine/bark.png"));
		ResourceManager.loadMesh("/prefabs/tree/pine/bark.obj", "bark", barkMat);
		
		Material leavesMat = new Material(TextureLoader.load("/prefabs/tree/pine/leaves.png"));
		leavesMat.setHasTransparency(true);
		ResourceManager.loadMesh("/prefabs/tree/pine/leaves.obj", "leaves", leavesMat);
		/*** *** ***/
		
		/*** Shroom ***/
		Material shroomMat = new Material(TextureLoader.load("/prefabs/shroom/shroom.png"));
		ResourceManager.loadMesh("/prefabs/shroom/shroom_1.obj", "shroom_1", shroomMat);
		ResourceManager.loadMesh("/prefabs/shroom/shroom_2.obj", "shroom_2", shroomMat);
		ResourceManager.loadMesh("/prefabs/shroom/shroom_3.obj", "shroom_3", shroomMat);
		/*** *** ***/
		
		/*** Flint ***/
		Material flintMat = new Material(TextureLoader.load("/prefabs/flint/flint.png"));
		ResourceManager.loadMesh("/prefabs/flint/flint.obj", "flint", flintMat);
		/*** *** ***/
		
		/*** Stick ***/
		Material stickMat = new Material(TextureLoader.load("/prefabs/stick/stick.png"));
		ResourceManager.loadMesh("/prefabs/stick/stick.obj", "stick", stickMat);
		/*** *** ***/
		
		/*** Wheat ***/
		Material wheatMat = new Material(TextureLoader.load("/prefabs/wheat/wheat.png"));
		wheatMat.setHasTransparency(true);
		wheatMat.setUseFakeLighting(true);
		ResourceManager.loadMesh("/prefabs/wheat/wheat.obj", "wheat", wheatMat);
		/*** *** ***/
		
		/*** Player ***/
		Material playerMat = new Material(ResourceManager.getTexture("skin"));
		
		ResourceManager.loadMesh("/prefabs/human/eyes.obj", "eyes", new Material(ResourceManager.getTexture("eyes")));
		
		ResourceManager.loadMesh("/prefabs/human/rightArm.obj", "rightArm", playerMat);
		ResourceManager.loadMesh("/prefabs/human/leftArm.obj", "leftArm", playerMat);
		ResourceManager.loadMesh("/prefabs/human/leftForearm.obj", "leftForearm", playerMat);
		ResourceManager.loadMesh("/prefabs/human/rightForearm.obj", "rightForearm", playerMat);
		ResourceManager.loadMesh("/prefabs/human/body.obj", "body", playerMat);
		ResourceManager.loadMesh("/prefabs/human/head.obj", "head", playerMat);
		ResourceManager.loadMesh("/prefabs/human/hip.obj", "hip", playerMat);
		ResourceManager.loadMesh("/prefabs/human/shin.obj", "shin", playerMat);
		/*** *** ***/
	}
	
}
