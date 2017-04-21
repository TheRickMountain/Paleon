package com.paleon.engine.graph.renderSystems;

import java.util.List;

import com.paleon.engine.components.Image;
import com.paleon.engine.components.Text;
import com.paleon.engine.items.GameObject;
import com.paleon.engine.toolbox.OpenglUtils;

public class GUIRendererSystem {

	public static ImageRendererSystem imageRendererSystem;
	public static FontRendererSystem fontRendererSystem;
	
	public GUIRendererSystem() {
		imageRendererSystem = new ImageRendererSystem();
		fontRendererSystem = new FontRendererSystem();
	}
	
	public void render(List<GameObject> gameObjects) {
		OpenglUtils.alphaBlending(true);
		OpenglUtils.depthTest(false);
		
		for(GameObject gameObject : gameObjects) {
			
			if(gameObject.getComponent(Image.class) != null) {
				// Render Image
			}
			
			if(gameObject.getComponent(Text.class) != null) {
				// Render Text
			}
			
		}
		
		OpenglUtils.alphaBlending(false);
		OpenglUtils.depthTest(true);
	}
	
}
