package com.paleon.astar;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
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
	
		for(Tile t : nodes.keySet()) {
			Node<Tile> n = nodes.get(t);
			
			List<PathEdge<Tile>> edges = new ArrayList<>();
			
			List<Tile> neighbours = t.getNeighbours();
			
			for(Tile tile : neighbours) {
				if(tile != null && tile.getMovementCost() > 0) {
					PathEdge<Tile> e = new PathEdge<>();
					e.cost = tile.getMovementCost();
					e.node = nodes.get(tile);
					
					edges.add(e);
				}
			}
			
			n.edges = new PathEdge[edges.size()];
			n.edges = edges.toArray(n.edges);
		}
		
	}
	
}
