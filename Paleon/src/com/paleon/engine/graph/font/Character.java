package com.paleon.engine.graph.font;

public class Character {

	private int id;
    private double xTextureCoord;
    private double yTextureCoord;
    private double xMaxTextureCoord;
    private double yMaxTextureCoord;
    private double xOffset;
    private double yOffset;
    private double sizeX;
    private double sizeY;
    private double xAdvance;
    
    protected Character(int id, double xTextureCoord, double yTextureCoord, double xTexSize, double yTexSize,
            double xOffset, double yOffset, double sizeX, double sizeY, double xAdvance) {
        this.id = id;
        this.xTextureCoord = xTextureCoord;
        this.yTextureCoord = yTextureCoord;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.xMaxTextureCoord = xTexSize + xTextureCoord;
        this.yMaxTextureCoord = yTexSize + yTextureCoord;
        this.xAdvance = xAdvance;
    }
 
    protected int getId() {
        return id;
    }
 
    protected double getxTextureCoord() {
        return xTextureCoord;
    }
 
    protected double getyTextureCoord() {
        return yTextureCoord;
    }
 
    protected double getXMaxTextureCoord() {
        return xMaxTextureCoord;
    }
 
    protected double getYMaxTextureCoord() {
        return yMaxTextureCoord;
    }
 
    protected double getxOffset() {
        return xOffset;
    }
 
    protected double getyOffset() {
        return yOffset;
    }
 
    protected double getSizeX() {
        return sizeX;
    }
 
    protected double getSizeY() {
        return sizeY;
    }
 
    protected double getxAdvance() {
        return xAdvance;
    }
	
}
