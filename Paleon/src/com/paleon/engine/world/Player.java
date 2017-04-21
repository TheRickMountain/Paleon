package com.paleon.engine.world;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.behaviour.CharacterAnimBh;
import com.paleon.engine.behaviour.CharacterControllerBh;
import com.paleon.engine.components.MeshFilter;
import com.paleon.engine.components.MeshRenderer;
import com.paleon.engine.graph.gui.Inventory;
import com.paleon.engine.items.Camera;
import com.paleon.engine.items.GameObject;
import com.paleon.scenes.World;

public class Player extends GameObject {

	public Player(Camera camera, World world, Inventory inventory) {
		this.addComponent(new MeshFilter(ResourceManager.getMesh("body")));
		this.addComponent(new MeshRenderer());		
		this.addComponent(new CharacterAnimBh());
		this.addComponent(new CharacterControllerBh(camera, world, inventory));
		this.position.set(400, 20, 400);
		
		{
			GameObject head = new GameObject("Head");
			head.addComponent(new MeshFilter(ResourceManager.getMesh("head")));
			head.addComponent(new MeshRenderer());
			head.localPosition.y = 0.85f;
			this.addChild(head);
			
			GameObject eyes = new GameObject("Eyes");
			eyes.addComponent(new MeshFilter(ResourceManager.getMesh("eyes")));
			eyes.addComponent(new MeshRenderer());
			eyes.localPosition.y = -0.1f;
			eyes.localRotation.y = 180;
			this.addChild(eyes);
			
			GameObject rightArm = new GameObject("RightArm");
			rightArm.addComponent(new MeshFilter(ResourceManager.getMesh("rightArm")));
			rightArm.addComponent(new MeshRenderer());
			rightArm.localPosition.set(0.65f, 0.6f, 0);
			this.addChild(rightArm);
			
			{
				GameObject rightForearm = new GameObject("RightForearm");
				rightForearm.addComponent(new MeshFilter(ResourceManager.getMesh("rightForearm")));
				rightForearm.addComponent(new MeshRenderer());
				rightForearm.localPosition.set(1.08f, -0.7f, 0);
				rightArm.addChild(rightForearm);
			}
			
			GameObject leftArm = new GameObject("LeftArm");
			leftArm.addComponent(new MeshFilter(ResourceManager.getMesh("leftArm")));
			leftArm.addComponent(new MeshRenderer());
			leftArm.localPosition.set(-0.65f, 0.6f, 0);
			this.addChild(leftArm);
			
			{
				GameObject leftForearm = new GameObject("LeftForearm");
				leftForearm.addComponent(new MeshFilter(ResourceManager.getMesh("leftForearm")));
				leftForearm.addComponent(new MeshRenderer());
				leftForearm.localPosition.set(-1.08f, -0.7f, 0);
				leftArm.addChild(leftForearm);
			}
			
			GameObject leftHip = new GameObject("LeftHip");
			leftHip.addComponent(new MeshFilter(ResourceManager.getMesh("hip")));
			leftHip.addComponent(new MeshRenderer());
			leftHip.localPosition.set(-0.4f, -0.45f, 0);
			this.addChild(leftHip);
			
			{
				GameObject leftShin = new GameObject("LeftShin");
				leftShin.addComponent(new MeshFilter(ResourceManager.getMesh("shin")));
				leftShin.addComponent(new MeshRenderer());
				leftShin.localPosition.set(0, -0.8f, 0);
				leftHip.addChild(leftShin);
			}
			
			GameObject rightHip = new GameObject("RightHip");
			rightHip.addComponent(new MeshFilter(ResourceManager.getMesh("hip")));
			rightHip.addComponent(new MeshRenderer());
			rightHip.localPosition.set(0.4f, -0.45f, 0);
			this.addChild(rightHip);
			
			{
				GameObject rightShin = new GameObject("RightShin");
				rightShin.addComponent(new MeshFilter(ResourceManager.getMesh("shin")));
				rightShin.addComponent(new MeshRenderer());
				rightShin.localPosition.set(0, -0.8f, 0);
				rightHip.addChild(rightShin);
			}
		}
	}
	
}
