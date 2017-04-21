package com.paleon.scenes;

import java.util.Random;

import com.paleon.engine.IScene;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.Resources;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.graph.gui.GUI;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TexturePack;
import com.paleon.engine.toolbox.GameTime;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.weather.Skybox;
import com.paleon.engine.world.Player;
import com.paleon.maths.vecmath.Vector3f;

public class Game implements IScene {

	public static enum State {
		GAME, INVENTORY
	}
	
	private Camera camera;
	
	private World world;
	
	public static State state;
	
	private float waterHeight = -1;

	public static GUI gui;
	
	Random rand = new Random();
	
	@Override
	public void init() {
		Resources.load();
		
		camera = new Camera(new Vector3f(400, 20, 400));
		
		world = new World(camera);
		
		TexturePack texturePack = new TexturePack("blendMap", "sand", "dirt", "grass", "grass");		
		Terrain terrain1 = new Terrain(0, 0, "/biomes/heightmap.png", texturePack);
		world.addTerrain(terrain1);
		
		
		for(int i = 60; i < 840; i+= 120) {
			for(int j = 60; j < 840; j+= 120) {
				world.addWaterTile(new WaterTile(j, waterHeight, i));
			}
		}
		
		GameTime.init();
		
		state = State.GAME;
		
		Mouse.hide();
		
		GameTime.setTime(6, 00);
	
		gui = new GUI();
		gui.init(world);
		
		world.setSkybox(new Skybox("sunset", "sunny2", "night"));
		
		/*** Environment generation ***/
		
		// Bush
		for(int i = 0; i < 20; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject gameObject = new GameObject();
				gameObject.addComponent(new MeshFilter(ResourceManager.getMesh("fern")));
				gameObject.position.set(x, world.getTerrainHeight(x, z), z);
				gameObject.scale.set(1);
				gameObject.setTextureIndex(rand.nextInt(4));
				gameObject.setFurthestPoint(ResourceManager.getMesh("fern").getFurthestPoint());
				gameObject.addComponent(new MeshRenderer());
				world.addGameObject(gameObject);
			}
		}
		
		// Flint
		for(int i = 0; i < 25; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject gameItem = new GameObject();
				gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("flint")));
				gameItem.position.set(x, world.getTerrainHeight(x, z), z);
				gameItem.scale.set(0.25f);
				gameItem.setId(MathUtils.generateId());
				gameItem.setItem(true);
				gameItem.setGuiId(4);
				gameItem.setFurthestPoint(ResourceManager.getMesh("flint").getFurthestPoint());
				gameItem.addComponent(new MeshRenderer());
				world.addGameObject(gameItem);
			}
		}
		
		// Shroom
		for(int i = 0; i < 50; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject gameItem = null;
				switch(rand.nextInt(3) + 1){
				case 1:
					gameItem = new GameObject();
					gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("shroom_1")));
					gameItem.setFurthestPoint(ResourceManager.getMesh("shroom_1").getFurthestPoint());
					break;
				case 2:
					gameItem = new GameObject();
					gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("shroom_2")));
					gameItem.setFurthestPoint(ResourceManager.getMesh("shroom_2").getFurthestPoint());
					break;
				case 3:
					gameItem = new GameObject();
					gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("shroom_3")));
					gameItem.setFurthestPoint(ResourceManager.getMesh("shroom_3").getFurthestPoint());
					break;
				}
				gameItem.position.set(x, world.getTerrainHeight(x, z), z);
				gameItem.scale.set(0.5f);
				gameItem.setId(MathUtils.generateId());
				gameItem.setItem(true);
				gameItem.setGuiId(1);
				gameItem.addComponent(new MeshRenderer());
				world.addGameObject(gameItem);
			}
		}
		
		// Stick
		for(int i = 0; i < 25; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject gameItem = new GameObject();
				gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("stick")));
				gameItem.position.set(x, world.getTerrainHeight(x, z), z);
				gameItem.scale.set(0.75f);
				gameItem.setId(MathUtils.generateId());
				gameItem.setItem(true);
				gameItem.setGuiId(2);
				gameItem.setFurthestPoint(ResourceManager.getMesh("stick").getFurthestPoint());
				gameItem.addComponent(new MeshRenderer());
				world.addGameObject(gameItem);
			}
		}
		
		// Wheat
		for(int i = 0; i < 25; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject gameItem = new GameObject();
				gameItem.addComponent(new MeshFilter(ResourceManager.getMesh("wheat")));
				gameItem.setUseWaving(true);
				gameItem.position.set(x, world.getTerrainHeight(x, z), z);
				gameItem.grass = true;
				gameItem.setId(MathUtils.generateId());
				gameItem.setItem(true);
				gameItem.setGuiId(3);
				gameItem.setFurthestPoint(ResourceManager.getMesh("wheat").getFurthestPoint());
				gameItem.addComponent(new MeshRenderer());
				world.addGameObject(gameItem);
			}
		}
		
		// Tree
		for(int i = 0; i < 100; i++) {
			float x = rand.nextFloat() * 800;
			float z = rand.nextFloat() * 800;
			if(world.getTerrainHeight(x,  z) > 0) {
				GameObject bark = new GameObject();
				bark.addComponent(new MeshFilter(ResourceManager.getMesh("bark")));
				bark.position.set(x, world.getTerrainHeight(x, z), z);
				bark.scale.set(3.5f);
				bark.setFurthestPoint(ResourceManager.getMesh("bark").getFurthestPoint());
				bark.addComponent(new MeshRenderer());
				world.addGameObject(bark);
				
				GameObject leaves = new GameObject();
				leaves.addComponent(new MeshFilter(ResourceManager.getMesh("leaves")));
				leaves.position.set(x, world.getTerrainHeight(x, z), z);
				leaves.scale.set(3.5f);
				leaves.setFurthestPoint(ResourceManager.getMesh("leaves").getFurthestPoint());
				leaves.addComponent(new MeshRenderer());
				world.addGameObject(leaves);
			}
		}
		
		/*** *** ***/
		
		Player player = new Player(camera, world, gui.getInventory());	
		world.addGameObject(player);
	}

	@Override
	public void update(float dt) {
		// Updating
		GameTime.update();
		
		if(state == State.GAME) {
			camera.update(dt);
		}
	
		world.update(dt);
		gui.update();
		
		// Rendering
		world.render(camera);
		gui.render();
	}

	@Override
	public void clear() {
		world.cleanup();
	}

}
