package com.paleon.scenes;

import java.util.Random;

import com.paleon.engine.IScene;
import com.paleon.engine.Resources;
import com.paleon.engine.input.Mouse;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.WaterTile;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TexturePack;
import com.paleon.engine.toolbox.GameTime;
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
		
		GameTime.setTime(12, 00);
	
		world.setSkybox(new Skybox("sunset", "sunny2", "night"));
		
		Player player = new Player(camera, world);	
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
		
		// Rendering
		world.render(camera);
	}

	@Override
	public void clear() {
		world.cleanup();
	}

}
