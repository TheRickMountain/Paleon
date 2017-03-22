package com.paleon.ecs;

import java.util.ArrayList;
import java.util.List;

import com.paleon.textures.Texture;
import com.paleon.utils.Color;

public class Entity {
	
	private String tag;
	
	private List<Component> components = new ArrayList<Component>();
	
	private Texture texture;	
	private final Color color;
	private boolean hasTexture = true;

	private float x;
	private float y;
	
	private float scaleX = 1.0f;
	private float scaleY = 1.0f;
	
	private boolean removed;
	
	private boolean walkable = true;
	
	public Entity(Texture texture) {
		this(texture, new Color());
	}
	
	public Entity(Color color) {
		this(null, color);
	}
	
	public Entity(Texture texture, Color color) {
		this.texture = texture;
		if(texture == null) {
			hasTexture = false;
		}
		this.color = color;
	}
	
	public void update(float dt) {
		for(Component component : components) {
			component.update(dt);
		}
	}
	
	public String getTag() {
		return tag;
	}

	public void setTag(String name) {
		this.tag = name;
	}
	
	public void setTexture(Texture texture) {
		this.texture = texture;
	}

	public Component addComponent(Component component) {
		this.components.add(component);
		return component;
	}
	
	public Component getComponent(ComponentType type) {
		for(Component component : components) {
			if(component.getType().equals(type)) {
				return component;
			}
		}
		return null;
	}
	
	public boolean hasComponent(ComponentType type) {
		for(Component component : components) {
			if(component.getType().equals(type)) {
				return true;
			}
		}
		
		return false;
	}
	
	public void removeComponent(ComponentType type) {
		for(Component component : components) {
			if(component.getType().equals(type)) {
				components.remove(component);
			}
		}
	}
	
	public Texture getTexture() {
		return texture;
	}
	
	public boolean isHasTexture() {
		return hasTexture;
	}

	public Color getColor() {
		return color;
	}
	
	public void setPosition(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public float getX() {
		return x;
	}

	public float getY() {
		return y;
	}
	
	public float getScaleX() {
		return scaleX;
	}
	
	public float getScaleY() {
		return scaleY;
	}
	
	public void setScale(float scale) {
		this.scaleX = scale;
		this.scaleY = scale;
	}
	
	public void setScale(float scaleX, float scaleY) {
		this.scaleX = scaleX;
		this.scaleY = scaleY;
	}
	
	public boolean isRemoved() {
		return removed;
	}
	
	public void setRemoved(boolean removed) {
		this.removed = removed;
	}
	
	public void remove() {
		removed = true;
	}

	public boolean isWalkable() {
		return walkable;
	}

	public void setWalkable(boolean value) {
		this.walkable = value;
	}
	
}
