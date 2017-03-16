package com.paleon.astar;

import java.util.HashMap;
import java.util.Map;

import com.paleon.core.World;
import com.paleon.terrain.Tile;

public class PathTileGraph {
	
	public Map<Tile, Node<Tile>> nodes;

	public PathTileGraph(World world) {
		nodes = new HashMap<>();
		
		for(int x = 0; x < world.getWidth(); x++) {
			for(int y = 0; y < world.getHeight(); y++) {
				Tile t = world.getTile(x, y);
				
				Node<Tile> n = new Node<>();
				n.data = t;
				nodes.put(t, n);
			}
		}
		
	}
	
}
