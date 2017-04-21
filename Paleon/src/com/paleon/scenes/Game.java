package com.paleon.scenes;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Random;

import com.paleon.engine.IScene;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.Resources;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.graph.gui.GUI;
import com.paleon.engine.input.Key;
import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.items.Player;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.items.animals.IAnimal;
import com.paleon.engine.items.animals.Sheep;
import com.paleon.engine.items.animals.SheepModel;
import com.paleon.engine.terrain.GrassRenderer;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TexturePack;
import com.paleon.engine.toolbox.GameTime;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.weather.Skybox;
import com.paleon.maths.vecmath.Vector3f;

public class Game implements IScene {

	public static enum State {
		GAME, INVENTORY
	}

	private boolean colorMode = false;
	
	private List<IAnimal> animals = new ArrayList<IAnimal>();
	
	private Camera camera;

	private Player player;
	
	private World world;
	
	public static State state;
	
	private float waterHeight = -1;

	public static GUI gui;
	
	GrassRenderer grassRenderer = new GrassRenderer();
	
	Random rand = new Random();
	
	@Override
	public void init() {
		Resources.load();
		
		camera = new Camera(new Vector3f(400, 20, 400));
		
		world = new World(camera);
		
		TexturePack texturePack = new TexturePack("blendMap", "sand", "dirt", "grass", "grass");		
		Terrain terrain1 = new Terrain(0, 0, "/biomes/heightmap.png", texturePack);
		world.addTerrain(terrain1);
		
		grassRenderer.init(camera.getProjectionMatrix(), generatePositionsForGrass(10000));
		
		player = new Player();
		player.setPosition(400, world.getTerrainHeight(400, 400), 400);
		player.add(world);
		
		for(int i = 60; i < 840; i+= 120) {
			for(int j = 60; j < 840; j+= 120) {
				world.addWaterTile(new WaterTile(j, waterHeight, i));
			}
		}
		
		SheepModel.init();
		Sheep sheep1 = new Sheep(world, 394, world.getTerrainHeight(394, 178), 178);
		sheep1.add(world);
		animals.add(sheep1);
		
		Sheep sheep2 = new Sheep(world, 374, world.getTerrainHeight(374, 198), 198);
		sheep2.add(world);
		animals.add(sheep2);
		
		GameTime.init();
		
		state = State.GAME;
		
		Mouse.hide();
		
		GameTime.setTime(16, 00);
	
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
				gameObject.addComponent(new MeshFilter(ResourceManager.getMesh("bush")));
				gameObject.position.set(x, world.getTerrainHeight(x, z), z);
				gameObject.setFurthestPoint(ResourceManager.getMesh("bush").getFurthestPoint());
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
				gameItem.setGuiId(0);
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
	}

	@Override
	public void update(float dt) {
		// Updating
		GameTime.update();
		
		world.update(dt);
		
		if(state == State.GAME) {
			player.move(dt, camera);
			player.setPosY(world.getTerrainHeight(player.getPosX(), player.getPosZ()) + 1.1f);
			player.update();
			
			camera.update(dt);
			camera.setPosition(player.getPosX(), player.getPosY() + 4.3f, player.getPosZ());
		}
	
		Iterator<IAnimal> iaIter = animals.iterator();
		while(iaIter.hasNext()){
			IAnimal animal = iaIter.next();
			animal.update(dt);
			/*if(Mouse.isButtonDown(0)){
				if(id == animal.getId()) {
					if(animal.getTag().equals("Sheep")) {
						if(Utils.getDistanceBetweenPoints(animal.getPosition(), player.getPosition()) < 7){
							player.attack(animal);
						}
					}
				} 
			}*/
			
		}
		
		Iterator<GameObject> giIter = world.getMeshRendererComponents().iterator();
		while(giIter.hasNext()){
			GameObject gameObject = giIter.next();
			gameObject.update();
			if(Keyboard.isKeyDown(Key.E)){
				if(world.getColorPickingId() == gameObject.getId()) {
					if(gameObject.isItem()) {
						if(MathUtils.getDistanceBetweenPoints(player.getPosition(), gameObject.position) <= 10) {
							if(gui.getInventory().addItem(gameObject.getGuiId())){
								gameObject.setRemove(true);
							}
						}
					}
				}
			}
			
			if(MathUtils.getDistanceBetweenPoints(gameObject.position, camera.getPosition()) >= 400) {
				gameObject.setFadeAway(true);
			} else {
				gameObject.setFadeAway(false);
			}
			
			if(gameObject.isRemove()) {
				giIter.remove();
			}
		}
		
		if(Keyboard.isKeyDown(Key.F6)) {
			colorMode = !colorMode;
		}
		
		gui.update();
		
		// Rendering
		world.render(camera, grassRenderer);
		gui.render();
	}

	@Override
	public void clear() {
		world.cleanup();
		grassRenderer.cleanup();
	}
	
	private float[] generatePositionsForGrass(int count) {
		float[] offset = new float[count * 3];
		
		int temp = 0;
		for(int i = 0; i < offset.length; i += 3) {
			Vector3f pos = generatePosition();
			if(!(pos.x == 0 && pos.y == 0 && pos.z == 0)) {
				temp = i;
				offset[temp] = pos.x;
				temp++;
				offset[temp] = pos.y;
				temp++;
				offset[temp] = pos.z;
			}
		}
	
		return offset;
	}
	
	private Vector3f generatePosition() {
		float x = rand.nextFloat() * 800;
		float z = rand.nextFloat() * 800;
		float y = world.getTerrainHeight(x, z);
		if(y < 0) {
			generatePosition();
		} else {
			return new Vector3f(x, y, z);
		}
		return new Vector3f(0, 0, 0);
	}

}
