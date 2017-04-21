package com.paleon.engine;

import java.util.HashMap;
import java.util.Map;

public class SceneManager implements IScene{

	private static Map<String, IScene> scenes;
	
	private static IScene currentScene;
	
	public SceneManager(){
		scenes = new HashMap<String, IScene>();
		currentScene = new EmptyScene();
		scenes.put(null, currentScene);
	}
	
	public static void add(String name, IScene scene) {
		scenes.put(name, scene);
    }
	
	public static void change(String name) {
        currentScene.clear();
        currentScene = scenes.get(name);
        currentScene.init();
    }
	
	@Override
	public void init() {
		currentScene.init();
	}

	@Override
	public void update(float deltaTime) {
		currentScene.update(deltaTime);
	}

	@Override
	public void clear() {
		currentScene.clear();
	}

}
