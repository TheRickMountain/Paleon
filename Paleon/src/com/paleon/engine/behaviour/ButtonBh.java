package com.paleon.engine.behaviour;

import com.paleon.engine.components.Behaviour;
import com.paleon.engine.input.Mouse;

public class ButtonBh extends Behaviour {
	
	@Override
	public void create() {
		
	}

	@Override
	public void update(float deltaTime) {
	
	}

	@Override
	public void onGui() {
		
	}
	
	public boolean isPressedDown(int button) {
		if(isOverMouse()) {
			if(Mouse.isButtonDown(button)) {
				return true;
			}
		}
		
		return false;
	}
	
	public boolean isPressedUp(int button) {
		if(isOverMouse()) {
			if(Mouse.isButtonUp(button)) {
				return true;
			}
		}
		
		return false;
	}

	public boolean isPressed(int button) {
		if(isOverMouse()) {
			if(Mouse.isButton(button)) {
				return true;
			}
		}
		
		return false;
	}
	
	public boolean isOverMouse() {
		float x = gameObject.position.x;
		float y = gameObject.position.y;
		float width = gameObject.getChildByName("Background").scale.x;
		float height = gameObject.getChildByName("Background").scale.y;
		
		if(Mouse.getX() >= x && Mouse.getX() <= x + width &&
				Mouse.getY() >= y && Mouse.getY() <= y + height) {
			return true;
		} else {
			return false;
		}
	}
	
}
