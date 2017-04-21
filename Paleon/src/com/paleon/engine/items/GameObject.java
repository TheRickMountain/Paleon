package com.paleon.engine.items;

import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.components.Component;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.graph.Material;
import com.paleon.engine.graph.Transform;
import com.paleon.engine.toolbox.ReflectionUtils;
import com.paleon.maths.vecmath.Vector3f;

public class GameObject {
	
	public Vector3f position = new Vector3f();
	public Vector3f rotation = new Vector3f();
	public Vector3f scale = new Vector3f(1, 1, 1);
	
	private List<Component> components;
	
	public final Transform transform;
	
	private boolean useWaving;
	
	public boolean grass = false;
	
	private int textureIndex = 0;
	
	private float distanceFromCamera = 10000;
	
	private boolean fadeAway = false;
	
	private int id = 0;
	
	private boolean remove = false;
	
	private boolean item = false;
	
	private int guiId = -1;
	
	private float furthestPoint = 1;
	
	public GameObject() {
		this.components = new ArrayList<Component>();
		this.transform = new Transform(this);
	}
	
	public void update() {
		transform.update();
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
		return remove;
	}

	public void setRemove(boolean remove) {
		this.remove = remove;
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
	
	public void addComponent(Component component) {
		component.setGameObject(this);
		this.components.add(component);
	}
	
	@SuppressWarnings("unchecked")
    public <T extends Component> T getComponent(Class<T> type)
    {
        for (Component component : components)
            if (ReflectionUtils.isInstanceOf(type, component))
                return (T) component;

        return null;
    }
}
