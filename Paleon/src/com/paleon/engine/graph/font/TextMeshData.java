package com.paleon.engine.graph.font;
public class TextMeshData {
     
    private float[] vertexPositions;
    
    protected TextMeshData(float[] vertexPositions){
        this.vertexPositions = vertexPositions;
    }
 
    public float[] getVertexPositions() {
        return vertexPositions;
    }
 
    public int getVertexCount() {
        return vertexPositions.length/4;
    }
 
}