package com.paleon.core;

import java.util.ArrayList;
import java.util.List;

import com.paleon.blueprints.EntityBlueprint;
import com.paleon.blueprints.RockBlueprint;
import com.paleon.blueprints.StoneBlueprint;
import com.paleon.blueprints.WoodBlueprint;
import com.paleon.components.Job;
import com.paleon.ecs.Entity;
import com.paleon.input.Mouse;
import com.paleon.instances.Pine;
import com.paleon.instances.Settler;
import com.paleon.math.Vector2f;
import com.paleon.terrain.Tile;
import com.paleon.textures.Texture;
import com.paleon.utils.MousePicker;
import com.paleon.utils.MyFile;

public class Game {

	private World world;
	
	private Tile firstTile;
	
	private Tile secondTile;
	private Tile lastTile;
	
	private List<Tile> selectedTiles = new ArrayList<Tile>();
	public static List<Tile> storageTiles = new ArrayList<Tile>();
	
	private Vector2f firstSelection;
	private Vector2f secondSelection;
	private Entity selection;
	
	//private Job.JobType jobType = Job.JobType.GATHERING;
	
	private StoneBlueprint stoneBp = new StoneBlueprint();
	private WoodBlueprint woodBp = new WoodBlueprint();
	private RockBlueprint rockBp = new RockBlueprint();
	private EntityBlueprint entityBp = new EntityBlueprint();
	
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
		
		
		world.addEntity(new Settler(world.getTile(10, 15)));
		/*world.addEntity(new Settler(world.getTile(16, 2)));
		world.addEntity(new Settler(world.getTile(2, 2)));*/
		
		world.getTile(4, 2).addEntity(new Pine());
		world.getTile(5, 6).addEntity(new Pine());
		world.getTile(6, 8).addEntity(new Pine());
		world.getTile(2, 5).addEntity(new Pine());
	}
	
	public void update(float dt) {
		/*if(Mouse.isButtonDown(1)) {
			world.getTile(MousePicker.getX(), MousePicker.getY()).addEntity(new Pine());
		}*/
		
		if(Mouse.isButtonDown(0)) {
			Tile tile = world.getTile(MousePicker.getX(), MousePicker.getY());
			if(tile.isHasEntity()) {
				world.jobList.push(new Job(tile, 0.5f));
			}
		}
		
		/*if(Keyboard.isKeyDown(Key.KEY_1)) {
			jobType = Job.JobType.GATHERING;
			System.out.println(jobType.toString());
		} else if(Keyboard.isKeyDown(Key.KEY_2)) {
			jobType = Job.JobType.BUILDING;
			System.out.println(jobType.toString());
		} else if(Keyboard.isKeyDown(Key.KEY_3)) {
			jobType = Job.JobType.MINING;
			System.out.println(jobType.toString());
		} else if(Keyboard.isKeyDown(Key.KEY_4)) {
			jobType = Job.JobType.CHOPPING;
			System.out.println(jobType.toString());
		} else if(Keyboard.isKeyDown(Key.KEY_5)) {
			jobType = Job.JobType.STORAGE_SELECTION;
			System.out.println(jobType.toString());
		}
		
		if(Mouse.isButtonDown(1)) {
			Tile tile = world.getTile(MousePicker.getX(), MousePicker.getY());
			List<Job> jobs = world.jobList;
			for(Job job : jobs) {
				if(job.getTile().equals(tile)) {
					job.getTile().removePrototype();
					jobs.remove(job);
					break;
				}
			}
		}
		
		switch(jobType) {
		case GATHERING:
		case CHOPPING:
		case MINING:
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
				int maxX = (int) Math.max(firstSelection.x, secondSelection.x);
				int maxY = (int) Math.max(firstSelection.y, secondSelection.y);
				
				int minX = (int) Math.min(firstSelection.x, secondSelection.x);
				int minY = (int) Math.min(firstSelection.y, secondSelection.y);
				
				for(int x = minX; x <= maxX; x++) {
					for(int y = minY; y <= maxY; y++) {
						Tile tile = world.getTile(x, y);
						if(tile.isHasEntity()) {
							switch(jobType) {
							case CHOPPING:
								InfoComponent ic = (InfoComponent)tile.getEntity().getComponent(ComponentType.INFO);
								if(ic.getInfoType().equals(InfoType.TREE)) {
									tile.addPrototype(new Cross());
									world.jobList.add(new Job(tile, jobType, 0.5f, woodBp));
								}
								break;
							case MINING:
								ic = (InfoComponent)tile.getEntity().getComponent(ComponentType.INFO);
								if(ic.getInfoType().equals(InfoType.ROCK)) {
									tile.addPrototype(new Cross());
									world.jobList.add(new Job(tile, jobType, 0.5f, rockBp));
								}
								break;
							case GATHERING:
								ic = (InfoComponent)tile.getEntity().getComponent(ComponentType.INFO);
								if(ic.getInfoType().equals(InfoType.RESOURCE)) {
									tile.addPrototype(new Cross());
									world.jobList.add(new Job(tile, jobType, 0.5f, null));
								}
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
			break;
		case BUILDING:
			if(Mouse.isButtonDown(0)) {
				firstTile = world.getTile(MousePicker.getX(), MousePicker.getY());
			}
			
			if(Mouse.isButton(0)) {
				if(firstTile != null) {
					secondTile = world.getTile(MousePicker.getX(), MousePicker.getY());
					
					if(!secondTile.equals(lastTile)) {
						// Unselect early area
						if(lastTile != null) {
							area(firstTile, lastTile, false, null, selectedTiles, null);
						}
						
						// Select new area
						area(firstTile, secondTile, true, stoneBp, selectedTiles, new Color(0.5f, 1.0f, 0.5f, 0.5f));
						
						lastTile = secondTile;
					}
					
				}
			}
			
			if(Mouse.isButtonUp(0)) {
				// Check red building
				boolean build = true;
				for(Tile tile : selectedTiles) {
					if(tile.getPrototype().getColor().r == 1.0f) {
						build = false;
						break;
					}
				}
				
				if(build) {
					for(Tile tile : selectedTiles) {
						world.jobList.add(new Job(tile, jobType, 0.5f, null));
					}
				} else {
					for(Tile tile : selectedTiles) {
						tile.removePrototype();
					}
				}
				
				firstTile = null;
				secondTile = null;
				lastTile = null;
				
				selectedTiles.clear();
			}
			break;
		case STORAGE_SELECTION:
			if(Mouse.isButtonDown(0)) {
				firstTile = world.getTile(MousePicker.getX(), MousePicker.getY());
			}
			
			if(Mouse.isButton(0)) {
				if(firstTile != null) {
					secondTile = world.getTile(MousePicker.getX(), MousePicker.getY());
					
					if(!secondTile.equals(lastTile)) {
						// Unselect early area
						if(lastTile != null) {
							area(firstTile, lastTile, false, null, selectedTiles, null);
						}
						
						// Select new area
						area(firstTile, secondTile, true, entityBp, selectedTiles, 
								new Color(82, 22, 180, 64).convert());
						
						lastTile = secondTile;
					}
					
				}
			}
			
			if(Mouse.isButtonUp(0)) {
				storageTiles.addAll(selectedTiles);
				
				firstTile = null;
				secondTile = null;
				lastTile = null;
				
				selectedTiles.clear();
			}
			break;
		}*/
		
	}
	
	/*private void area(Tile t1, Tile t2, boolean select, Blueprint blueprint, List<Tile> list, Color color) {
		int maxX = Math.max(t1.getX(), t2.getX());
		int maxY = Math.max(t1.getY(), t2.getY());
		
		int minX = Math.min(t1.getX(), t2.getX());
		int minY = Math.min(t1.getY(), t2.getY());
		
		for(int x = minX; x <= maxX; x++) {
			for(int y = minY; y <= maxY; y++) {
				Tile tile = world.getTile(x, y);
				if(select) {
					Entity entity = blueprint.getInstance();
					if(!tile.isHasEntity()) {
						entity.getColor().set(color.r, color.g, color.b, color.a);
					} else {
						entity.getColor().set(1.0f, 0.5f, 0.5f, 0.5f);
					}
					tile.addPrototype(entity);
					list.add(tile);
				} else {
					tile.removePrototype();
					list.clear();
				}
			}
		}
	}*/
	
}
