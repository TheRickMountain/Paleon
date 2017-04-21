package com.paleon.engine.toolbox;

import java.nio.ByteBuffer;

import org.lwjgl.BufferUtils;
import org.lwjgl.opengl.GL11;

import com.paleon.engine.Display;

public class ColorPicker {

	public static int getId(float x, float y){
		ByteBuffer colors = BufferUtils.createByteBuffer(3);
		GL11.glReadPixels((int)x, Display.getHeight() - (int)y - 1, 
				1, 1, GL11.GL_RGBA, GL11.GL_UNSIGNED_BYTE, colors);
		int red = colors.get(0);
		int green = colors.get(1);
		int blue = colors.get(2);
		if(red < 0){
			red += 256;
		}
		if(green < 0){
			green += 256;
		}
		if(blue < 0){
			blue += 256;
		}
		return MathUtils.getIdByColor(red, green, blue);
	}
	
}
