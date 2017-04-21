package com.paleon.engine.loaders;

import static com.paleon.engine.toolbox.IOUtil.ioResourceToByteBuffer;
import static org.lwjgl.opengl.GL11.GL_LINEAR_MIPMAP_LINEAR;
import static org.lwjgl.opengl.GL11.GL_RGBA;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_2D;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_MIN_FILTER;
import static org.lwjgl.opengl.GL11.GL_UNPACK_ALIGNMENT;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_BYTE;
import static org.lwjgl.opengl.GL11.glBindTexture;
import static org.lwjgl.opengl.GL11.glGenTextures;
import static org.lwjgl.opengl.GL11.glPixelStorei;
import static org.lwjgl.opengl.GL11.glTexImage2D;
import static org.lwjgl.opengl.GL11.glTexParameterf;
import static org.lwjgl.opengl.GL11.glTexParameteri;
import static org.lwjgl.opengl.GL30.glGenerateMipmap;
import static org.lwjgl.stb.STBImage.stbi_failure_reason;
import static org.lwjgl.stb.STBImage.stbi_image_free;
import static org.lwjgl.stb.STBImage.stbi_info_from_memory;
import static org.lwjgl.stb.STBImage.stbi_load_from_memory;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.IntBuffer;

import org.lwjgl.BufferUtils;
import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL12;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL14;

public class TextureLoader {
	
	public static int load(BufferedImage image) {
		int[] pixels = new int[image.getWidth() * image.getHeight()];
		image.getRGB(0, 0, image.getWidth(), image.getHeight(), pixels, 0, image.getWidth());
		
		ByteBuffer buffer = BufferUtils.createByteBuffer(image.getWidth() * image.getHeight() * 4);
		for(int y = 0; y < image.getHeight(); y++){
            for(int x = 0; x < image.getWidth(); x++){
                int pixel = pixels[y * image.getWidth() + x];
                buffer.put((byte) ((pixel >> 16) & 0xFF));     // Red component
                buffer.put((byte) ((pixel >> 8) & 0xFF));      // Green component
                buffer.put((byte) (pixel & 0xFF));               // Blue component
                buffer.put((byte) ((pixel >> 24) & 0xFF));    // Alpha component. Only for RGBA
            }
        }
		buffer.flip();
		
		int id = glGenTextures();
        // Bind the texture
        glBindTexture(GL_TEXTURE_2D, id);

        // Tell OpenGL how to unpack the RGBA bytes. Each component is 1 byte size
        glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

        // Upload the texture data
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, image.getWidth(), image.getHeight(), 0, GL_RGBA, GL_UNSIGNED_BYTE, buffer);
        
        // Generate Mip Map
        glGenerateMipmap(GL_TEXTURE_2D);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameterf(GL_TEXTURE_2D, GL14.GL_TEXTURE_LOD_BIAS, -0.4f);
        
        return id;
	}
	
	public static int load(String texturePath) {	
		TextureData data = decodeTextureFile(texturePath);
		
		int id = glGenTextures();
        // Bind the texture
        glBindTexture(GL_TEXTURE_2D, id);

        // Tell OpenGL how to unpack the RGBA bytes. Each component is 1 byte size
        glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

        // Upload the texture data
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, data.getWidth(), data.getHeight(), 0, GL_RGBA, GL_UNSIGNED_BYTE, data.getBuffer());
        
        glGenerateMipmap(GL_TEXTURE_2D);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameterf(GL_TEXTURE_2D, GL14.GL_TEXTURE_LOD_BIAS, -0.4f);
		
        stbi_image_free(data.getBuffer());
        
		return id;
	}
	
    public static int loadCubemap(String skyboxName) throws Exception {
    	int id = GL11.glGenTextures();
    	GL13.glActiveTexture(GL13.GL_TEXTURE0);
    	GL11.glBindTexture(GL13.GL_TEXTURE_CUBE_MAP, id);
    	
    	String[] skyboxNames = new String[]{
    			"right.png", "left.png", "top.png", "bottom.png", "back.png", "front.png"};
    	
    	for(int i = 0; i < skyboxNames.length; i++) {
    		TextureData data = decodeTextureFile("/skyboxes/" + skyboxName + "/" + skyboxNames[i]);
    		GL11.glTexImage2D(GL13.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL11.GL_RGBA, 
    				data.getWidth(), data.getHeight(), 0, GL11.GL_RGBA, GL11.GL_UNSIGNED_BYTE, 
    				data.getBuffer());
    	}
    	GL11.glTexParameteri(GL13.GL_TEXTURE_CUBE_MAP, GL11.GL_TEXTURE_MAG_FILTER, GL11.GL_LINEAR);
    	GL11.glTexParameteri(GL13.GL_TEXTURE_CUBE_MAP, GL11.GL_TEXTURE_MIN_FILTER, GL11.GL_LINEAR);
    	GL11.glTexParameteri(GL13.GL_TEXTURE_CUBE_MAP, GL11.GL_TEXTURE_WRAP_S, GL12.GL_CLAMP_TO_EDGE);
    	GL11.glTexParameteri(GL13.GL_TEXTURE_CUBE_MAP, GL11.GL_TEXTURE_WRAP_T, GL12.GL_CLAMP_TO_EDGE);
    	return id;
    }
    
    private static TextureData decodeTextureFile(String texturePath) {
    	ByteBuffer imageBuffer;
		try {
			imageBuffer = ioResourceToByteBuffer(texturePath, 8 * 1024);
		} catch (IOException e) {
			throw new RuntimeException(e);
		}

		IntBuffer w = BufferUtils.createIntBuffer(1);
		IntBuffer h = BufferUtils.createIntBuffer(1);
		IntBuffer comp = BufferUtils.createIntBuffer(1);

		// Use info to read image metadata without decoding the entire image.
		// We don't need this for this demo, just testing the API.
		if ( !stbi_info_from_memory(imageBuffer, w, h, comp) )
			throw new RuntimeException("Failed to read image information: " + stbi_failure_reason());

		// Decode the image
		ByteBuffer image = stbi_load_from_memory(imageBuffer, w, h, comp, 0);
		if ( image == null )
			throw new RuntimeException("Failed to load image: " + stbi_failure_reason());
		
		return new TextureData(image, w.get(0), h.get(0));
    }
    
}
