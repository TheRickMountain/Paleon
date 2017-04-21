package com.paleon.test;

import java.awt.image.BufferedImage;
import java.awt.image.BufferedImageOp;
import java.awt.image.ConvolveOp;
import java.awt.image.Kernel;

public class ImageUtils {

	public static BufferedImage blur(BufferedImage image) {
		float[] matrix = {
			    1/16f, 1/8f, 1/16f, 
			    1/8f, 1/4f, 1/8f, 
			    1/16f, 1/8f, 1/16f, 
		};
		
		BufferedImageOp op = new ConvolveOp( new Kernel(3, 3, matrix), ConvolveOp.EDGE_NO_OP, null);
		
		return op.filter(image, null);
	}
	
}
