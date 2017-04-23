package com.paleon.engine;

import org.lwjgl.opengl.GL11;

import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;
import com.paleon.scenes.Game;
import com.paleon.scenes.Menu;

public class PaleonEngine implements Runnable {
	
    private final Thread gameLoopThread;

    private SceneManager scene;
	
    public PaleonEngine() {
        gameLoopThread = new Thread(this, "GAME_LOOP_THREAD");
        this.scene = new SceneManager();
    }
    
    public void start() {
        String osName = System.getProperty("os.name");
        if ( osName.contains("Mac") ) {
            gameLoopThread.run();
        } else {
            gameLoopThread.start();
        }
    }
    
    @Override
    public void run() {
    	try {
            init();
            gameLoop();
            dispose();
        } catch (Exception excp) {
            excp.printStackTrace();
        }
    }
    
    protected void init() throws Exception{
    	Display.create("Paleon 0.1", 1152, 648);
    	initScenes();
    }
    
    public void initScenes() throws Exception{
		SceneManager.add("Menu", new Menu());
		SceneManager.add("Game", new Game());
		SceneManager.change("Menu");
	}
    
    public void gameLoop() throws Exception {
        while (!Display.isCloseRequested()) {
            Keyboard.startEventFrame();
    		Mouse.startEventFrame();
            
    		float deltaTime = Time.getDeltaTime();
    		if(deltaTime >= 1){ 
    			deltaTime = 0;
    		}
    		
    		update(deltaTime);
           
            Keyboard.clearEventFrame();
            Mouse.clearEventFrame();
            
            if(Display.wasResized()) {
    			GL11.glViewport(0, 0, Display.getWidth(), Display.getHeight());
    			Display.setResized(false);
            }
            
        }
    }
    
    protected void update(float deltaTime) throws Exception {
    	Display.preUpdate();
    	scene.update(deltaTime); 	
    	Display.postUpdate();
    	Time.update();
    }
    
    public void dispose() throws Exception{
    	SceneManager.change(null);
    }
  
}
