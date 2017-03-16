package com.paleon.astar;

import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.Stack;

import com.paleon.core.World;
import com.paleon.terrain.Tile;
import com.paleon.utils.Heap;

public class PathAStar {
	
	private Stack<Tile> path;
	
	private World world;
	
	public PathAStar(World world, Tile tileStart, Tile tileEnd) {
		this.world = world;
		if(world.getTileGraph() == null) {
			world.setTileGraph(new PathTileGraph(world));
		}
		
		Map<Tile, Node<Tile>> nodes = world.getTileGraph().nodes;
		
		if(!nodes.containsKey(tileStart) || !nodes.containsKey(tileEnd)) {
			System.out.println("Node hasn't start or end tile");
			return;
		}
		
		Node<Tile> startNode = nodes.get(tileStart);
		Node<Tile> targetNode = nodes.get(tileEnd);
		
		Heap<Node<Tile>> openSet = new Heap<>(nodes.size());	
		Set<Node<Tile>> closedSet = new HashSet<>();
		openSet.add(startNode);
		
		while(openSet.getCount() > 0) {
			Node<Tile> currentNode = openSet.removeFirst();
			closedSet.add(currentNode);
			
			if(currentNode.equals(targetNode)) {
				retracePath(startNode, targetNode);
				return;
			}
			
			for(Tile n : currentNode.data.getNeighbours(true)) {
				Node<Tile> neighbourNode = nodes.get(n);
				
				if(n == null) {
					continue;
				}
				
				if(isClippingCorner(currentNode.data, n)) {
					continue;
				}
				
				if(n.getMovementCost() == 0 || closedSet.contains(neighbourNode)) {
					continue;
				}
				
				int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbourNode);
				if(newMovementCostToNeighbour < neighbourNode.gCost || !openSet.contains(neighbourNode)) {
					neighbourNode.gCost = newMovementCostToNeighbour;
					neighbourNode.hCost = getDistance(neighbourNode, targetNode);
					neighbourNode.parent = currentNode;
					
					if(!openSet.contains(neighbourNode)) {
						openSet.add(neighbourNode);
					} else {
						openSet.updateItem(neighbourNode);
					}
				}
			}
		}
	}
	
	private void retracePath(Node<Tile> startNode, Node<Tile> endNode) {
		path = new Stack<>();
		Node<Tile> currentNode = endNode;
		 
		while(!currentNode.equals(startNode)) {
			path.push(currentNode.data);
			currentNode = currentNode.parent;
		}
	}
	
	public int getLength() {
		if(path == null) {
			return 0;
		}
		
		return path.size();
	}
	
	public Tile getNextTile() {
		return path.pop();
	}
	
	private int getDistance(Node<Tile> a, Node<Tile> b) {
		int dstX = Math.abs(a.data.getX() - b.data.getX());
		int dstY = Math.abs(a.data.getY() - b.data.getY());
		
		if(dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		} else {
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
	
	private boolean isClippingCorner(Tile curr, Tile neigh) {
		int dX = curr.getX() - neigh.getX();
		int dY = curr.getY() - neigh.getY();
		
		if(Math.abs(dX) + Math.abs(dY) == 2) {
			if(world.getTile(curr.getX() - dX, curr.getY()).getMovementCost() == 0) {
				return true;
			}
			
			if(world.getTile(curr.getX(), curr.getY() - dY).getMovementCost() == 0) {
				return true;
			}
		}
		
		return false;
	}

}
