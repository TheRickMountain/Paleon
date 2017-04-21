package com.paleon.engine.items;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.components.Component;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.Behaviour;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.toolbox.MathUtils;
import com.paleon.engine.toolbox.ReflectionUtils;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;

public class GameObject {
	
	public String name;
	
	public Vector3f position = new Vector3f();
	public Vector3f rotation = new Vector3f();
	public Vector3f scale = new Vector3f(1, 1, 1);
	
	public Vector3f localPosition = new Vector3f();
	public Vector3f localRotation = new Vector3f();
	public Vector3f localScale = new Vector3f(0, 0, 0);
	
	private List<Component> components = new ArrayList<Component>();
	private List<Behaviour> behaviours = new ArrayList<Behaviour>();
	private List<GameObject> children = new ArrayList<GameObject>();
	
	private GameObject parent;
	
	private boolean active = true;
	
	public final Transform transform;
	
	private boolean useWaving;
	
	public boolean grass = false;
	
	private int textureIndex = 0;
	
	private float distanceFromCamera = 10000;
	
	private boolean fadeAway = false;
	
	private int id = 0;
	
	private boolean removed = false;
	
	private boolean item = false;
	
	private int guiId = -1;
	
	private float furthestPoint = 1;
	
	public GameObject() {
		this("");
	}
	
	public GameObject(String name) {
		this.name = name;
		this.transform = new Transform(this);
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
		for(GameObject child : children) {
			child.update(deltaTime);
		}
		
		updatePRS();
		
		transform.update();
		
		for(Behaviour b : behaviours) {
			b.update(deltaTime);
		}
	}
	
	public float getTextureXOffset(){
		Material material = getComponent(MeshFilter.class).mesh.getMaterial();
		
		int column = textureIndex % material.getNumberOfRows();
		return (float) column / (float) material.getNumberOfRows();
	}
	
	public float getTextureYOffset(){
		Material material = getComponent(MeshFilter.class).mesh.getMaterial();
		
		int row = textureIndex / material.getNumberOfRows();
		return (float) row / (float)material.getNumberOfRows();
	}

	public boolean isUseWaving() {
		return useWaving;
	}

	public void setUseWaving(boolean useWaving) {
		this.useWaving = useWaving;
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
		
		for(GameObject child : children) {
			child.remove();
		}
	}

	public boolean isItem() {
		return item;
	}

	public void setItem(boolean item) {
		this.item = item;
	}

	public int getGuiId() {
		return guiId;
	}

	public void setGuiId(int guiId) {
		this.guiId = guiId;
	}
	
	public void setFurthestPoint(float furthestPoint) {
		this.furthestPoint = furthestPoint;
	}
	
	public float getFurthestPoint() {
		float val = furthestPoint * Math.max(Math.max(scale.x, scale.y), scale.z);
		return val;
	}
	
	public void addChild(GameObject gameObject) {
		gameObject.setParent(this);
		children.add(gameObject);
	}
	
	public void removeChild(GameObject gameObject) {
		gameObject.setParent(null);
		children.remove(gameObject);
	}
	
	public List<GameObject> getChildren() {
		return children;
	}
	
	public GameObject getChildByName(String name) {
		for(GameObject gameObject : children) {
			if(gameObject.name == name) {
				return gameObject;
			}
		}
		return null;
	}
	
	public void addComponent(Component component) {
		component.setGameObject(this);
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
	
	public GameObject getParent() {
		return parent;
	}
	
	private void setParent(GameObject parent) {
		this.parent = parent;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
		
		for(GameObject child : children) {
			child.setActive(active);
		}
	}

}
