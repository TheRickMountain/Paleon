package com.paleon.engine.toolbox;

import java.nio.ByteBuffer;
import java.util.Objects;

public class Color {
	
	public static final Color SUNSET = new Color(198, 150, 90);
	public static final Color CLOUDY = new Color(108, 137, 151);
	public static final Color SUNNY = new Color(203, 221, 223);
	public static final Color NIGHT = new Color(0, 0, 0);
	
	public static final Color BLACK = new Color(0x000000FF);
    public static final Color WHITE = new Color(0xFFFFFFFF);
    public static final Color BLUE = new Color(0x0000FFFF);
    public static final Color GREEN = new Color(0x00FF00FF);
    public static final Color RED = new Color(0xFF0000FF);
    public static final Color GREY = new Color(0x888888FF);
    public static final Color TRANSPARENT = new Color(0x00000000);
    public static final Color YELLOW = new Color(0xFFFF00FF);
    public static final Color CYAN = new Color(0x00FFFFFF);
    public static final Color MAGENTA = new Color(0xFF00FFFF);
    public static final Color ORANGE = new Color(255, 165, 0);
	public static final Color PURPLE = new Color(125, 0, 125);

    private static final int MAX = 255;
    private static final int RED_OFFSET = 24;
    private static final int GREEN_OFFSET = 16;
    private static final int BLUE_OFFSET = 8;
    private static final int RED_FILTER = 0x00FFFFFF;
    private static final int GREEN_FILTER = 0xFF00FFFF;
    private static final int BLUE_FILTER = 0xFFFF00FF;
    private static final int ALPHA_FILTER = 0xFFFFFF00;

    private int rgba;

    /**
     * Creates a color that is black with full alpha.
     */
    public Color() {
        rgba = 0x000000FF;
    }

    public Color(int representation) {
        this.rgba = representation;
    }

    /**
     * Create a color with the given red/green/blue values. Alpha is initialised as max.
     *
     * @param r
     * @param g
     * @param b
     */
    public Color(float r, float g, float b) {
        this((int) (r * MAX), (int) (g * MAX), (int) (b * MAX));
    }

    /**
     * Creates a color with the given red/green/blue/alpha values.
     *
     * @param r
     * @param g
     * @param b
     * @param a
     */
    public Color(float r, float g, float b, float a) {
        this((int) (r * MAX), (int) (g * MAX), (int) (b * MAX), (int) (a * MAX));
    }

    /**
     * Creates a color with the given red/green/blue values. Alpha is initialised as max.
     *
     * @param r
     * @param g
     * @param b
     */
    public Color(int r, int g, int b) {
        this(r, g, b, 0xFF);
    }

    /**
     * Creates a color with the given red/green/blue/alpha values.
     *
     * @param r
     * @param g
     * @param b
     * @param a
     */
    public Color(int r, int g, int b, int a) {
        Preconditions.checkArgument(r >= 0 && r <= MAX, "Color values must be in range 0-255");
        Preconditions.checkArgument(g >= 0 && g <= MAX, "Color values must be in range 0-255");
        Preconditions.checkArgument(b >= 0 && b <= MAX, "Color values must be in range 0-255");
        Preconditions.checkArgument(a >= 0 && a <= MAX, "Color values must be in range 0-255");
        rgba = r << RED_OFFSET | g << GREEN_OFFSET | b << BLUE_OFFSET | a;
    }

    /**
     * @return The red component, between 0 and 255
     */
    public int getR() {
        return (rgba >> RED_OFFSET) & MAX;
    }

    /**
     * @return The green component, between 0 and 255
     */
    public int getG() {
        return (rgba >> GREEN_OFFSET) & MAX;
    }

    /**
     * @return The blue component, between 0 and 255
     */
    public int getB() {
        return (rgba >> BLUE_OFFSET) & MAX;
    }

    /**
     * @return The alpha component, between 0 and 255
     */
    public int getA() {
        return rgba & MAX;
    }

    public float getRf() {
        return getR() / 255.f;
    }

    public float getGf() {
        return getG() / 255.f;
    }
    
    public float getBf() {
        return getB() / 255.f;
    }

    public float getAf() {
        return getA() / 255.f;
    }
    
    public void set(int r, int g, int b) {
    	setR(r); setG(g); setB(b);
    }
    
    public void set(float r, float g, float b) {
    	setRf(r); setGf(g); setBf(b);
    }

    public void setR(int value) {
    	Preconditions.checkArgument(value >= 0 && value <= MAX, "Color values must be in range 0-255");
        rgba = value << RED_OFFSET | (rgba & RED_FILTER);
    }

    public void setB(int value) {
        Preconditions.checkArgument(value >= 0 && value <= MAX, "Color values must be in range 0-255");
        rgba = value << BLUE_OFFSET | (rgba & BLUE_FILTER);
    }

    public void setG(int value) {
        Preconditions.checkArgument(value >= 0 && value <= MAX, "Color values must be in range 0-255");
        rgba = value << GREEN_OFFSET | (rgba & GREEN_FILTER);
    }

    public void setA(int value) {
        Preconditions.checkArgument(value >= 0 && value <= MAX, "Color values must be in range 0-255");
        rgba = value | (rgba & ALPHA_FILTER);
    }
    
    public void setRf(float value) {
    	setR((int) (value * MAX));
    }
    
    public void setGf(float value) {
    	setG((int) (value * MAX));
    }
    
    public void setBf(float value) {
    	setB((int) (value * MAX));
    }
    
    public void setAf(float value) {
    	setA((int) (value * MAX));
    }

    public void inverse() {
        rgba = (~rgba & ALPHA_FILTER) | getA();
    }

    public int getRGBA() {
        return rgba;
    }

    public void addToBuffer(ByteBuffer buffer) {
        buffer.putInt(rgba);
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == this) {
            return true;
        }
        if (obj instanceof Color) {
            Color other = (Color) obj;
            return rgba == other.rgba;
        }
        return false;
    }

    @Override
    public int hashCode() {
        return Objects.hash(rgba);
    }

    public int toHex() {
    	return (int) getR() * 65536 + (int) getG() * 256 + (int) getB();
    }

    @Override
    public String toString() {
        return getR() + " " + getG() + " " + getB() + " " + getA();
    }
	
}
