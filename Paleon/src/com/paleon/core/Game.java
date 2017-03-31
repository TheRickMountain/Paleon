package com.paleon.core;

import com.paleon.input.Mouse;
import com.paleon.instances.Entity;
import com.paleon.instances.Pine;
import com.paleon.instances.Settler;
import com.paleon.instances.Stone;
import com.paleon.instances.Wood;
import com.paleon.terrain.Tile;
import com.paleon.textures.Texture;
import com.paleon.utils.Color;
import com.paleon.utils.MousePicker;
import com.paleon.utils.MyFile;

public class Game {

	private World world;
	
	public static JobType jobType = JobType.STOCKPILE;
	
	public Game() {
		world = World.getInstance();
		
		ResourceManager.loadTexture("settler", Texture.newTexture(new MyFile("sprites/settler.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("pine", Texture.newTexture(new MyFile("sprites/pine.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("stone", Texture.newTexture(new MyFile("sprites/stone.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("wheat", Texture.newTexture(new MyFile("sprites/wheat.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("cross", Texture.newTexture(new MyFile("sprites/cross.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("wood", Texture.newTexture(new MyFile("sprites/wood.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("rock", Texture.newTexture(new MyFile("sprites/rock.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("building", Texture.newTexture(new MyFile("gui/building.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("chopping", Texture.newTexture(new MyFile("gui/chopping.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("gardening", Texture.newTexture(new MyFile("gui/gardening.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("gathering", Texture.newTexture(new MyFile("gui/gathering.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("mining", Texture.newTexture(new MyFile("gui/mining.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("storage", Texture.newTexture(new MyFile("gui/storage.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("fishing", Texture.newTexture(new MyFile("gui/fishing.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("fish", Texture.newTexture(new MyFile("sprites/fish.png"))
				.normalMipMap().build());
		
		ResourceManager.loadTexture("bed", Texture.newTexture(new MyFile("sprites/bed.png"))
				.normalMipMap().build());
		
		for(int i = 0; i < 8; i++) {
			ResourceManager.loadTexture("wheat_stage_" + i, Texture.newTexture(new MyFile("wheat/wheat_stage_" + i + ".png"))
					.normalMipMap().build());
		}		
		
		for(int i = 18; i < 25; i++) {
			for(int j = 18; j < 25; j++) {
				world.getTile(i, j).addEntityToWorld(new Stone());
			}
		}
		
		world.camera.setPosition(16, 16);
		
		world.addEntity(new Settler(world, world.getTile(18, 16)));
		
		world.getTile(4, 2).addEntityToWorld(new Pine());
		world.getTile(5, 2).addEntityToWorld(new Pine());
		world.getTile(4, 3).addEntityToWorld(new Pine());
		world.getTile(5, 6).addEntityToWorld(new Pine());
		world.getTile(6, 8).addEntityToWorld(new Pine());
		world.getTile(2, 5).addEntityToWorld(new Pine());
		
		world.getTile(14, 12).setId(5);
		world.getTile(15, 16).setId(5);
		world.getTile(16, 18).setId(5);
		world.getTile(12, 15).setId(5);
		
		world.getTile(5, 22).addEntityToWorld(new Wood());
		
	}
	
	public void update(float dt) {		
		if(Mouse.isButtonDown(0)) {
			Tile tile = world.getTile(MousePicker.getX(), MousePicker.getY());
			switch (jobType) {
			case PRODUCTION:
				world.jobList.add(new Job(tile, jobType));
				break;
			case STOCKPILE:
				tile.addPrototype(new Entity(new Color(150, 0, 205, 32).convert()));
				break;
			case GATHERING:
				world.jobList.add(new Job(tile, jobType));
				break;
			}
		}
	}
	
}
