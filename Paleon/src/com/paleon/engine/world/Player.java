package com.paleon.engine.world;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.behaviour.CharacterAnimBh;
import com.paleon.engine.behaviour.CharacterControllerBh;
import com.paleon.engine.graph.Material;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.Entity;
import com.paleon.scenes.World;

public class Player extends Entity {

	public Player(Camera camera, World world) {
		super(ResourceManager.getMesh("body"), new Material(ResourceManager.getTexture("skin")));
				
		this.addComponent(new CharacterAnimBh());
		this.addComponent(new CharacterControllerBh(camera, world));
		this.position.set(400, 20, 400);
		
		{
			Entity head = new Entity("Head", ResourceManager.getMesh("head"),
					new Material(ResourceManager.getTexture("skin")));
			head.localPosition.y = 0.85f;
			this.addChild(head);
			
			Entity eyes = new Entity("Eyes", ResourceManager.getMesh("eyes"), 
					new Material(ResourceManager.getTexture("eyes")));
			eyes.getMaterial().setReflectivity(1);
			eyes.getMaterial().setShineDamper(10);
			eyes.localPosition.y = -0.1f;
			eyes.localRotation.y = 180;
			this.addChild(eyes);
			
			Entity rightArm = new Entity("RightArm", ResourceManager.getMesh("rightArm"), getMaterial());
			rightArm.localPosition.set(0.65f, 0.6f, 0);
			this.addChild(rightArm);
			
			{
				Entity rightForearm = new Entity("RightForearm", ResourceManager.getMesh("rightForearm"),
						getMaterial());
				rightForearm.localPosition.set(1.08f, -0.7f, 0);
				rightArm.addChild(rightForearm);
			}
			
			Entity leftArm = new Entity("LeftArm", ResourceManager.getMesh("leftArm"), getMaterial());
			leftArm.localPosition.set(-0.65f, 0.6f, 0);
			this.addChild(leftArm);
			
			{
				Entity leftForearm = new Entity("LeftForearm", ResourceManager.getMesh("leftForearm"),
						getMaterial());
				leftForearm.localPosition.set(-1.08f, -0.7f, 0);
				leftArm.addChild(leftForearm);
			}
			
			Entity leftHip = new Entity("LeftHip", ResourceManager.getMesh("hip"), getMaterial());
			leftHip.localPosition.set(-0.4f, -0.45f, 0);
			this.addChild(leftHip);
			
			{
				Entity leftShin = new Entity("LeftShin", ResourceManager.getMesh("shin"), getMaterial());
				leftShin.localPosition.set(0, -0.8f, 0);
				leftHip.addChild(leftShin);
			}
			
			Entity rightHip = new Entity("RightHip", ResourceManager.getMesh("hip"), getMaterial());
			rightHip.localPosition.set(0.4f, -0.45f, 0);
			this.addChild(rightHip);
			
			{
				Entity rightShin = new Entity("RightShin", ResourceManager.getMesh("shin"), getMaterial());
				rightShin.localPosition.set(0, -0.8f, 0);
				rightHip.addChild(rightShin);
			}
		}
	}
	
}
