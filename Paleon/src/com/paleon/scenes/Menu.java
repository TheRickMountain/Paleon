package com.paleon.scenes;

import com.paleon.engine.IScene;
import com.paleon.engine.SceneManager;

public class Menu implements IScene {
	
	@Override
	public void init() {
		//SceneManager.change("Game");
	}

	@Override
	public void update(float dt) {
		SceneManager.change("Game");
	}
	
	@Override
	public void clear() {
		
	}
	

}
