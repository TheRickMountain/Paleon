package com.paleon.core;

import static org.lwjgl.glfw.Callbacks.glfwFreeCallbacks;
import static org.lwjgl.glfw.GLFW.GLFW_CONTEXT_VERSION_MAJOR;
import static org.lwjgl.glfw.GLFW.GLFW_CONTEXT_VERSION_MINOR;
import static org.lwjgl.glfw.GLFW.GLFW_FALSE;
import static org.lwjgl.glfw.GLFW.GLFW_OPENGL_CORE_PROFILE;
import static org.lwjgl.glfw.GLFW.GLFW_OPENGL_FORWARD_COMPAT;
import static org.lwjgl.glfw.GLFW.GLFW_OPENGL_PROFILE;
import static org.lwjgl.glfw.GLFW.GLFW_RESIZABLE;
import static org.lwjgl.glfw.GLFW.GLFW_TRUE;
import static org.lwjgl.glfw.GLFW.GLFW_VISIBLE;
import static org.lwjgl.glfw.GLFW.glfwCreateWindow;
import static org.lwjgl.glfw.GLFW.glfwDefaultWindowHints;
import static org.lwjgl.glfw.GLFW.glfwDestroyWindow;
import static org.lwjgl.glfw.GLFW.glfwGetPrimaryMonitor;
import static org.lwjgl.glfw.GLFW.glfwGetVideoMode;
import static org.lwjgl.glfw.GLFW.glfwInit;
import static org.lwjgl.glfw.GLFW.glfwMakeContextCurrent;
import static org.lwjgl.glfw.GLFW.glfwPollEvents;
import static org.lwjgl.glfw.GLFW.glfwSetCursorPosCallback;
import static org.lwjgl.glfw.GLFW.glfwSetErrorCallback;
import static org.lwjgl.glfw.GLFW.glfwSetKeyCallback;
import static org.lwjgl.glfw.GLFW.glfwSetMouseButtonCallback;
import static org.lwjgl.glfw.GLFW.glfwSetScrollCallback;
import static org.lwjgl.glfw.GLFW.glfwSetWindowPos;
import static org.lwjgl.glfw.GLFW.glfwSetWindowSizeCallback;
import static org.lwjgl.glfw.GLFW.glfwShowWindow;
import static org.lwjgl.glfw.GLFW.glfwSwapBuffers;
import static org.lwjgl.glfw.GLFW.glfwSwapInterval;
import static org.lwjgl.glfw.GLFW.glfwTerminate;
import static org.lwjgl.glfw.GLFW.glfwWindowHint;
import static org.lwjgl.glfw.GLFW.glfwWindowShouldClose;
import static org.lwjgl.opengl.GL11.GL_TRUE;
import static org.lwjgl.system.MemoryUtil.NULL;

import java.util.ArrayList;
import java.util.List;

import org.lwjgl.glfw.GLFW;
import org.lwjgl.glfw.GLFWErrorCallback;
import org.lwjgl.glfw.GLFWVidMode;
import org.lwjgl.glfw.GLFWWindowSizeCallback;
import org.lwjgl.opengl.GL;
import org.lwjgl.opengl.GL11;

import com.paleon.input.Keyboard;
import com.paleon.input.Mouse;
import com.paleon.input.Scroll;
import com.paleon.toolbox.Utils;

/**
 * Created by Rick on 06.10.2016.
 */
public class Display {

	private static final int ROLLING_AVERAGE_LENGTH = 10;
	private static final float DELTA_FACTOR = 1000;
	
    private static long window;
    private String title = "";
    private static boolean isResized;
    private static int width;
    private static int height;
    
    private float delta = 0;
	private long lastFrame = 0;
	private List<Float> previousTimes = new ArrayList<Float>();
	
	private boolean fullscreen = false;
	
	public static long defaultCursor;
	public static long takeCursor;
	public static long takeNonactiveCursor;
	
	private static long currentCursor;

    public Display(String title, int width, int height, boolean fullscreen) {
        this.title = title;
    	Display.width = width;
        Display.height = height;
        this.fullscreen = fullscreen;
        
        init();
    }
        
    private void init() {
        if(!glfwInit())
            throw new IllegalStateException("Unable to initialize GLFW");

        GLFWErrorCallback.createPrint(System.err).set();
        
        glfwDefaultWindowHints();
        glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
        glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
        glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
        glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
        glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);
        glfwWindowHint(GLFW_RESIZABLE, GLFW_TRUE);
        //glfwWindowHint(GLFW_SAMPLES, 4);

        if(fullscreen) {
        	GLFWVidMode vidmode = glfwGetVideoMode(glfwGetPrimaryMonitor());
        	Display.width = vidmode.width();
        	Display.height = vidmode.height();
        	window = glfwCreateWindow(Display.width, Display.height, title, glfwGetPrimaryMonitor(), NULL);
        } else {
        	window = glfwCreateWindow(width, height, title, NULL, NULL);
        }
        
        if ( window == NULL )
            throw new RuntimeException("Failed to create the GLFW window");
        
        GLFWVidMode vidmode = glfwGetVideoMode(glfwGetPrimaryMonitor());
        glfwSetWindowPos(
                window,
                (vidmode.width() - width) / 2,
                (vidmode.height() - height) / 2);

        glfwMakeContextCurrent(window);
        
        glfwSwapInterval(1);

        glfwShowWindow(window);

        GL.createCapabilities();
        
        glfwSetWindowSizeCallback(window, new GLFWWindowSizeCallback() {
            @Override
            public void invoke(long window, int width, int height) {
                resize(width, height);
            }
        });
        
        glfwSetKeyCallback(window, new Keyboard());
        glfwSetMouseButtonCallback(window, new Mouse(this));
        glfwSetCursorPosCallback(window, new Mouse.Cursor());
        glfwSetScrollCallback(window, new Scroll());
        
        lastFrame = getTime();
    }
    
    public void resize(int width, int height) {
    	Display.width = width;
    	Display.height = height;
    	setResized(true);
    	GL11.glViewport(0, 0, Display.width, Display.height);
    }

    public boolean isCloseRequested() {
        return glfwWindowShouldClose(window);
    }
    
    public void pollEvents() {
    	glfwPollEvents();
    }

    public void swapBuffers() {
    	glfwSwapBuffers(window);
    	calculateDelta();
    }

    public static int getWidth() {
        return width;
    }

    public static int getHeight() {
        return height;
    }
    
    public float getDeltaInSeconds() {
    	return delta;
    }

    public static boolean isResized() {
        return isResized;
    }

    public void setResized(boolean resized) {
        Display.isResized = resized;
    }

    public long getWindow() {
        return window;
    }
    
    private void calculateDelta() {
    	long time = getTime();
		long difference = time - lastFrame;
		float value = ((float) difference) / DELTA_FACTOR;
		delta = updateRollingAverage(value);
		lastFrame = time;
    }
    
    private float updateRollingAverage(float value) {
		previousTimes.add(0, value);
		if (previousTimes.size() > ROLLING_AVERAGE_LENGTH) {
			previousTimes.remove(ROLLING_AVERAGE_LENGTH);
		}
		if (previousTimes.size() < ROLLING_AVERAGE_LENGTH) {
			return value;
		}
		return Utils.getAverageOfList(previousTimes);
	}
    
    private long getTime() {
    	return System.nanoTime() / 1000000;
    }

    public void shutdown() {
        glfwFreeCallbacks(window);
        glfwDestroyWindow(window);
        glfwTerminate();
        glfwSetErrorCallback(null).free();
    }
    
    public static void setCursor(long cursor) {
    	if(currentCursor != cursor) {
    		currentCursor = cursor;
    		GLFW.glfwSetCursor(window, cursor);
    	}
    }
}
