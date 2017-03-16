package com.paleon.components;

import com.paleon.astar.PathAStar;
import com.paleon.core.World;
import com.paleon.ecs.Component;
import com.paleon.ecs.ComponentType;
import com.paleon.ecs.Entity;
import com.paleon.input.Mouse;
import com.paleon.terrain.Tile;
import com.paleon.utils.MathUtils;
import com.paleon.utils.MousePicker;

public class SettlerComponent extends Component {
	
	private Tile currTile;
	
	private Tile nextTile;
	private Tile destTile;
	
	private PathAStar pathAStar;
	
	private float movementPerc;	
	private float speed = 2f;
	
	public SettlerComponent(Tile tile, Entity parent) {
		super(parent);
		currTile = destTile = nextTile = tile;
		parent.setPosition(tile.getX(), tile.getY());	
	}
	
	@Override
	public void update(float dt) {
		if(Mouse.isButtonDown(0)) {
			Tile tile = World.getInstance().getTile(MousePicker.getX(), MousePicker.getY());
			pathAStar = new PathAStar(World.getInstance(), currTile, tile);
			
			if(pathAStar.getLength() > 0) {
				destTile = tile;
			}
		}
		
		move(dt);
	}

	private void move(float dt) {
		if(currTile.equals(destTile)) {
			pathAStar = null;
			return;
		}
		
		if(nextTile.equals(currTile)) {
			nextTile = pathAStar.getNextTile();
		}
		
		float distToTravel = MathUtils.getDistance(
				currTile.getX(), currTile.getY(), 
				nextTile.getX(), nextTile.getY());
		
		float distThisFrame = speed * dt;
		
		float percThisFrame = distThisFrame / distToTravel;
		
		movementPerc += percThisFrame;
		
		if(movementPerc >= 1) {			
			currTile = nextTile;
			movementPerc = 0;
		}
		
		parent.setPosition(
				MathUtils.getLerp(currTile.getX(), nextTile.getX(), movementPerc), 
				MathUtils.getLerp(currTile.getY(), nextTile.getY(), movementPerc));
	}

	@Override
	public ComponentType getType() {
		return ComponentType.SETTLER;
	}

}
