package com.paleon.engine;

import static org.lwjgl.glfw.Callbacks.glfwFreeCallbacks;
import static org.lwjgl.glfw.GLFW.*;
import static org.lwjgl.opengl.GL11.GL_TRUE;
import static org.lwjgl.system.MemoryUtil.NULL;

import org.lwjgl.Version;
import org.lwjgl.glfw.GLFW;
import org.lwjgl.glfw.GLFWErrorCallback;
import org.lwjgl.glfw.GLFWVidMode;
import org.lwjgl.glfw.GLFWWindowSizeCallback;
import org.lwjgl.opengl.GL;
import org.lwjgl.opengl.GL11;

import com.paleon.engine.input.Keyboard;
import com.paleon.engine.input.Mouse;

public class Display {

	private static long window;
	
	private static int width;
	private static int height;
	
	private static boolean resized;
	
	public static void create(String title, int newWidth, int newHeight) {
		width = newWidth;
		height = newHeight;
		
		GLFWErrorCallback.createPrint(System.err).set();
		
		if ( !glfwInit() )
			throw new IllegalStateException("Unable to initialize GLFW");
		
		glfwDefaultWindowHints(); 
		glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE); 
		glfwWindowHint(GLFW_RESIZABLE, GLFW_TRUE);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
        glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 2);
        glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
        glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
		
		window = glfwCreateWindow(width, height, title, NULL, NULL);
		if ( window == NULL )
			throw new RuntimeException("Failed to create the GLFW window");
		
		
		glfwSetWindowSizeCallback(window, new GLFWWindowSizeCallback() {
            @Override
            public void invoke(long window, int width, int height) {
                Display.width = width;
                Display.height = height;
                setResized(true);
            }
        });
		
		glfwSetKeyCallback(window, new Keyboard()); 
		glfwSetMouseButtonCallback(window, new Mouse()); 
		glfwSetCursorPosCallback(window, new Mouse.Cursor()); 
		glfwSetScrollCallback(window, new Mouse.Scroll());
		
		GLFWVidMode vidmode = glfwGetVideoMode(glfwGetPrimaryMonitor());
		glfwSetWindowPos(
				window,
				(vidmode.width() - width) / 2,
				(vidmode.height() - height) / 2);

		glfwMakeContextCurrent(window);
		glfwSwapInterval(1);
		
		glfwShowWindow(window);
		
		GL.createCapabilities();
		
		System.out.println("LWJGL " + Version.getVersion());
		System.out.println("OpenGL " + GL11.glGetString(GL11.GL_VERSION));
	}
	
	public static boolean isCloseRequested() {
		return GLFW.glfwWindowShouldClose(window);
	}
	
	public static void preUpdate() {
		GLFW.glfwPollEvents();
	}
	
	public static void postUpdate() {
		GLFW.glfwSwapBuffers(window);
	}

	public static int getWidth() {
		return width;
	}

	public static int getHeight() {
		return height;
	}
	
	public static boolean wasResized() {
        return resized;
    }

    public static void setResized(boolean resized) {
        Display.resized = resized;
    }
    
    public static long getWindow() {
    	return window;
    }
	
	public static void destroy() {
		glfwFreeCallbacks(window);
		glfwDestroyWindow(window);
		glfwTerminate();
		glfwSetErrorCallback(null).free();
	}
	
}
