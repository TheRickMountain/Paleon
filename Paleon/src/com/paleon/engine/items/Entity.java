package com.paleon.engine.items;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.components.Behaviour;
import com.paleon.engine.components.Component;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.ReflectionUtils;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class Entity {
	
	public String name;
	
	public Vector3f position = new Vector3f();
	public Vector3f rotation = new Vector3f();
	public Vector3f scale = new Vector3f(1, 1, 1);
	
	public Vector3f localPosition = new Vector3f();
	public Vector3f localRotation = new Vector3f();
	public Vector3f localScale = new Vector3f(0, 0, 0);
	
	private List<Component> components = new ArrayList<Component>();
	private List<Behaviour> behaviours = new ArrayList<Behaviour>();
	private List<Entity> children = new ArrayList<Entity>();
	
	private Mesh mesh;
	private Material material;
	
	private Entity parent;
	
	private boolean active = true;
	
	public final Transform transform;
	
	public boolean grass = false;
	
	private int textureIndex = 0;
	
	private float distanceFromCamera = 10000;
	
	private boolean fadeAway = false;
	
	private int id = 0;
	
	private boolean removed = false;
	
	private float furthestPoint = 1;
	
	public Entity(Mesh mesh, Material material) {
		this("", mesh, material);
	}
	
	public Entity(String name, Mesh mesh, Material material) {
		this.name = name;
		this.transform = new Transform(this);
		this.mesh = mesh;
		this.material = material;
	}
	
	public void init() {
		for(Behaviour b : behaviours) {
			b.create();
		}
		
		updatePRS();
	}
	
	private void updatePRS() {
		if(getParent() != null) {
			Matrix4f modelMatrix = MathUtils.getEulerModelMatrix(getParent().position, getParent().rotation, getParent().scale);
			Vector4f newPos = Matrix4f.transform(modelMatrix, new Vector4f(localPosition.x, localPosition.y, localPosition.z, 1.0f), null);
			
			position.set(newPos);
			rotation.set(Vector3f.add(localRotation,  getParent().rotation, null));
			scale.set(Vector3f.add(localScale,  getParent().scale, null));
		}
	}
	
	public void update(float deltaTime) {
		for(Entity child : children) {
			child.update(deltaTime);
		}
		
		updatePRS();
		
		transform.update();
		
		for(Behaviour b : behaviours) {
			b.update(deltaTime);
		}
	}
	
	public float getTextureXOffset(){	
		int column = textureIndex % material.getNumberOfRows();
		return (float) column / (float) material.getNumberOfRows();
	}
	
	public float getTextureYOffset(){
		int row = textureIndex / material.getNumberOfRows();
		return (float) row / (float)material.getNumberOfRows();
	}

	public int getTextureIndex() {
		return textureIndex;
	}

	public void setTextureIndex(int textureIndex) {
		this.textureIndex = textureIndex;
	}

	public float getFadeAwayDistance() {
		return distanceFromCamera;
	}

	public void setFadeAwayDistance(float fadeAwayDistance) {
		this.distanceFromCamera = fadeAwayDistance;
	}

	public boolean isFadeAway() {
		return fadeAway;
	}

	public void setFadeAway(boolean fadeAway) {
		this.fadeAway = fadeAway;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public boolean isRemove() {
		return removed;
	}

	public void remove() {
		this.removed = true;
		
		for(Entity child : children) {
			child.remove();
		}
	}
	
	public void setFurthestPoint(float furthestPoint) {
		this.furthestPoint = furthestPoint;
	}
	
	public float getFurthestPoint() {
		float val = furthestPoint * Math.max(Math.max(scale.x, scale.y), scale.z);
		return val;
	}
	
	public void addChild(Entity gameObject) {
		gameObject.setParent(this);
		children.add(gameObject);
	}
	
	public void removeChild(Entity gameObject) {
		gameObject.setParent(null);
		children.remove(gameObject);
	}
	
	public List<Entity> getChildren() {
		return children;
	}
	
	public Entity getChildByName(String name) {
		for(Entity gameObject : children) {
			if(gameObject.name == name) {
				return gameObject;
			}
		}
		return null;
	}
	
	public void addComponent(Component component) {
		component.setEntity(this);
		this.components.add(component);
	}
	
	public void addComponent(Behaviour behaviour) {
		behaviour.setGameObject(this);
		this.behaviours.add(behaviour);
	}
	
	public void removeComponent(Component component) {
		this.components.remove(component);
	}
	
	@SuppressWarnings("unchecked")
    public <T extends Component> T getComponent(Class<T> type)
    {
        for (Component component : components)
            if (ReflectionUtils.isInstanceOf(type, component))
                return (T) component;

        return null;
    }
	
	@SuppressWarnings("unchecked")
    public <T extends Behaviour> T getBehaviour(Class<T> type)
    {
        for (Behaviour behaviour : behaviours)
            if (ReflectionUtils.isInstanceOf(type, behaviour))
                return (T) behaviour;

        return null;
    }
	
	public Entity getParent() {
		return parent;
	}
	
	private void setParent(Entity parent) {
		this.parent = parent;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
		
		for(Entity child : children) {
			child.setActive(active);
		}
	}
	
	public Mesh getMesh() {
		return mesh;
	}
	
	public void setMesh(Mesh mesh) {
		this.mesh = mesh;
	}
	
	public Material getMaterial() {
		return material;
	}
	
	public void setMaterial(Material material) {
		this.material = material;
	}

}
