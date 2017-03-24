package com.paleon.core;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.paleon.blueprints.Blueprint;
import com.paleon.blueprints.EntityBlueprint;
import com.paleon.blueprints.StoneBlueprint;
import com.paleon.components.InfoComponent;
import com.paleon.components.InfoType;
import com.paleon.components.Job;
import com.paleon.components.SettlerComponent;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.input.Key;
import com.paleon.input.Keyboard;
import com.paleon.input.Mouse;
import com.paleon.instances.Bed;
import com.paleon.instances.Pine;
import com.paleon.instances.Settler;
import com.paleon.instances.Stone;
import com.paleon.math.Vector2f;
import com.paleon.terrain.Tile;
import com.paleon.textures.Texture;
import com.paleon.utils.Color;
import com.paleon.utils.MousePicker;
import com.paleon.utils.MyFile;

public class Game {

	private World world;
	
	private Tile firstTile;
	
	private Tile secondTile;
	private Tile lastTile;
	
	private List<Tile> selectedTiles = new ArrayList<Tile>();
	public static Map<Tile, Integer> storageTiles = new HashMap<>();
	
	public List<Garden> gardens = new ArrayList<>();

	public static Map<Integer, Texture> wheat = new HashMap<>();
	
	public static Map<Bed, Settler> beds = new HashMap<>();
	
	private Vector2f firstSelection;
	private Vector2f secondSelection;
	private Entity selection;
	
	private StoneBlueprint stoneBp = new StoneBlueprint();
	
	private EntityBlueprint entityBp = new EntityBlueprint();
	
	public static JobType jobType = JobType.PRODUCTION;
	
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
			// Wheat stages initialization
			wheat.put(i, ResourceManager.getTexture("wheat_stage_" + i));
		}		
		
		for(int i = 18; i < 25; i++) {
			for(int j = 18; j < 25; j++) {
				world.getTile(i, j).setId(5);
			}
		}
		
		
		world.addEntity(new Settler(world.getTile(18, 16)));
		//world.addEntity(new Settler(world.getTile(16, 2)));
		//world.addEntity(new Settler(world.getTile(10, 2)));
		
		world.getTile(4, 2).addEntityToWorld(new Pine());
		world.getTile(5, 6).addEntityToWorld(new Pine());
		world.getTile(6, 8).addEntityToWorld(new Pine());
		world.getTile(2, 5).addEntityToWorld(new Pine());
		
		world.getTile(14, 12).addEntityToWorld(new Stone());
		world.getTile(15, 16).addEntityToWorld(new Stone());
		world.getTile(16, 18).addEntityToWorld(new Stone());
		world.getTile(12, 15).addEntityToWorld(new Stone());
		
	}
	
	public void update(float dt) {		
		if(!Mouse.isActiveInGUI()) {
			switch(jobType){
			case GATHERING:
			case PRODUCTION:
				entitySelection();
				break;
			case STORAGE:
				storageSelection();
				break;
			case BUILDING:
				buildingSelection();
				break;
			case PLOWING:
				gardenSelection();
				break;
			case FISHING:
				waterSelection();
				break;
			case FURNITURE:
				furniturePlace();
			}
		}
		
		// TODO: Rewrite
		if(Keyboard.isKeyDown(Key.KEY_H)) {
			jobType = JobType.FURNITURE;
			System.out.println(jobType.toString());
		}
		
		for(Garden garden : gardens) {
			garden.update();
		}
	}
	
	private void area(Tile t1, Tile t2, List<Tile> list, Blueprint blueprint, Color color, boolean prototype) {
		int maxX = Math.max(t1.getX(), t2.getX());
		int maxY = Math.max(t1.getY(), t2.getY());
		
		int minX = Math.min(t1.getX(), t2.getX());
		int minY = Math.min(t1.getY(), t2.getY());
		
		for(int x = minX; x <= maxX; x++) {
			for(int y = minY; y <= maxY; y++) {
				Tile tile = world.getTile(x, y);
				Entity entity = blueprint.getInstance();
				if(!tile.isHasEntity()) {
					entity.getColor().set(color.r, color.g, color.b, color.a);
				} else {
					entity.getColor().set(1.0f, 0.5f, 0.5f, 0.5f);
				}
				if(prototype) {
					tile.addPrototype(entity);
				} else {
					tile.addEntityToWorld(entity);
				}
				list.add(tile);
			}
		}
	}
	
	private void entitySelection() {
		if(Mouse.isButtonDown(0)) {
			selection = new Entity(new Color(1.0f, 1.0f, 1.0f, 0.25f));
			selection.setPosition(MousePicker.getXf(), MousePicker.getYf());
			world.addEntity(selection);
			
			firstSelection = new Vector2f(selection.getX(), selection.getY());
			secondSelection = new Vector2f(selection.getX(), selection.getY());
		}
		
		if(Mouse.isButton(0)) {
			if(firstSelection != null) {
				secondSelection.set(MousePicker.getXf(), MousePicker.getYf());
				
				selection.setScale(secondSelection.x - firstSelection.x,
						secondSelection.y - firstSelection.y);
			}
		}
		
		if(Mouse.isButtonUp(0)) {
			if(firstSelection != null) {
				int maxX = (int) Math.max(firstSelection.x, secondSelection.x);
				int maxY = (int) Math.max(firstSelection.y, secondSelection.y);
				
				int minX = (int) Math.min(firstSelection.x, secondSelection.x);
				int minY = (int) Math.min(firstSelection.y, secondSelection.y);
				
				for(int x = minX; x <= maxX; x++) {
					for(int y = minY; y <= maxY; y++) {
						Tile tile = world.getTile(x, y);
						if(tile.isHasEntity()) {
							InfoComponent ic = (InfoComponent)tile.getEntity().getComponent(ComponentType.INFO);
							switch(jobType) {
							case PRODUCTION:
								if(ic.getInfoType().equals(InfoType.PRODUCTION)) {
									world.jobList.add(new Job(tile, 0.5f, jobType, ic.getProductionResource()));
								}
								break;
							case GATHERING:
								if(ic.getInfoType().equals(InfoType.GATHERING)) {
									world.jobList.add(new Job(tile, 0.0f, jobType, null));
								}
								break;
							case BUILDING:
								break;
							case STORAGE:
								break;
							default:
								break;
							}
						}
					}
				}
				
				firstSelection = null;
				secondSelection = null;
				selection.remove();
				selection = null;
			}
		}
	}
	
	private void storageSelection() {
		if(Mouse.isButtonDown(0)) {
			firstTile = world.getTile(MousePicker.getX(), MousePicker.getY());
		}
		
		if(Mouse.isButton(0)) {
			if(firstTile != null) {
				secondTile = world.getTile(MousePicker.getX(), MousePicker.getY());
				
				if(!secondTile.equals(lastTile)) {
					// Unselect early area
					if(lastTile != null) {
						for(Tile tile : selectedTiles) {
							tile.removePrototype();
						}
						selectedTiles.clear();
					}
					
					// Select new area
					area(firstTile, secondTile, selectedTiles, entityBp, 
							new Color(82, 22, 180, 64).convert(), true);
					
					lastTile = secondTile;
				}
				
			}
		}
		
		if(Mouse.isButtonUp(0)) {
			if(firstTile != null) {
				for(Tile tile : selectedTiles) {
					storageTiles.put(tile, 0);
				}
				
				firstTile = null;
				secondTile = null;
				lastTile = null;
				
				selectedTiles.clear();
				}
		}
	}
	
	private void buildingSelection() {
		if(Mouse.isButtonDown(0)) {
			firstTile = world.getTile(MousePicker.getX(), MousePicker.getY());
		}
		
		if(Mouse.isButton(0)) {
			if(firstTile != null) {
				secondTile = world.getTile(MousePicker.getX(), MousePicker.getY());
				
				if(!secondTile.equals(lastTile)) {
					// Unselect early area
					if(lastTile != null) {
						for(Tile tile : selectedTiles) {
							tile.removeEntityFromWorld();
						}
						selectedTiles.clear();
					}
					
					// Select new area
					area(firstTile, secondTile, selectedTiles, stoneBp, 
							new Color(0.5f, 1.0f, 0.5f, 0.5f), false);
					
					lastTile = secondTile;
				}
				
			}
		}
		
		if(Mouse.isButtonUp(0)) {
			if(firstTile != null) {
				for(Tile tile : selectedTiles) {
					world.jobList.add(new Job(tile, 0.5f, jobType, new Stone()));
				}
				
				firstTile = null;
				secondTile = null;
				lastTile = null;
				
				for(Entity entity : world.settlersList) {
					SettlerComponent sc = (SettlerComponent) entity.getComponent(ComponentType.SETTLER);
					sc.updatePathfinding();
				}
				
				selectedTiles.clear();
			}
		}
	}
	
	private void gardenSelection() {
		if(Mouse.isButtonDown(0)) {
			firstTile = world.getTile(MousePicker.getX(), MousePicker.getY());
		}
		
		if(Mouse.isButton(0)) {
			if(firstTile != null) {
				secondTile = world.getTile(MousePicker.getX(), MousePicker.getY());
				
				if(!secondTile.equals(lastTile)) {
					// Unselect early area
					if(lastTile != null) {
						for(Tile tile : selectedTiles) {
							tile.removePrototype();
						}
						selectedTiles.clear();
					}
					
					// Select new area
					area(firstTile, secondTile, selectedTiles, entityBp, 
							new Color(0.5f, 1.0f, 0.5f, 0.15f), true);
					
					lastTile = secondTile;
				}
				
			}
		}
		
		if(Mouse.isButtonUp(0)) {
			if(firstTile != null) {
				gardens.add(new Garden(selectedTiles));
		
				firstTile = null;
				secondTile = null;
				lastTile = null;
				
				selectedTiles.clear();
			}
		}
	}
	
	private void waterSelection() {
		if(Mouse.isButtonDown(0)) {
			Tile tile = world.getTile(MousePicker.getX(), MousePicker.getY());
			if(tile.getId() == 5) {
				world.jobList.add(new Job(tile, 0.0f, JobType.FISHING, null));
			}
		}
	}
	
	private void furniturePlace() {
		if(Mouse.isButtonDown(0)) {
			Tile tile = world.getTile(MousePicker.getX(), MousePicker.getY());
			if(tile.getId() != 5 && !tile.isHasEntity()) {
				Bed bed = new Bed();
				tile.addEntityToWorld(bed);
				
				Settler settler = null;
				for(Entity e : world.settlersList) {
					SettlerComponent sc = (SettlerComponent) e.getComponent(ComponentType.SETTLER);
					if(sc.getBed() == null) {
						sc.setBed(bed);
						settler = (Settler) e;
					}
				}
				
				beds.put(bed, settler);
			}
		}
	}
	
}
