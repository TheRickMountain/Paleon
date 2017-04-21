package com.paleon.engine.graph.renderSystems;

import java.util.List;
import java.util.Map;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.Display;
import com.paleon.engine.ResourceManager;
import com.paleon.engine.components.Text;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.graph.font.FontType;
import com.paleon.engine.items.GameObject;
import com.paleon.maths.vecmath.Vector2f;

public class FontRendererSystem {
 
    private ShaderProgram shader;
 
    public FontRendererSystem() {
        shader = ResourceManager.loadShader("font");
        
        shader.createUniform("translation");
        shader.createUniform("color");
    }
     
    public void render(Map<FontType, List<GameObject>> texts){
        prepare();
        for(FontType font : texts.keySet()){
            GL13.glActiveTexture(GL13.GL_TEXTURE0);
            GL11.glBindTexture(GL11.GL_TEXTURE_2D, font.getTextureAtlas());
            for(GameObject text : texts.get(font)) {
            	render(text);
            }
        }
        endRendering();
    }
 
    public void cleanup(){
        shader.cleanup();
    }
     
    private void prepare(){
        GL11.glEnable(GL11.GL_BLEND);
        GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
        GL11.glDisable(GL11.GL_DEPTH_TEST);
        shader.bind();
    }
     
    private void render(GameObject gameObject){
    	Text text = gameObject.getComponent(Text.class);
    	
        GL30.glBindVertexArray(text.textMeshVao);
        GL20.glEnableVertexAttribArray(0);
        GL20.glEnableVertexAttribArray(1);
        shader.setUniform("translation", new Vector2f(gameObject.position.x / Display.getWidth(), 
        		gameObject.position.y / Display.getHeight()));
        shader.setUniform("color", text.color);
        GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, text.vertexCount);
        GL20.glDisableVertexAttribArray(0);
        GL20.glDisableVertexAttribArray(1);
        GL30.glBindVertexArray(0);
    }
     
    private void endRendering(){
        shader.unbind();
        GL11.glDisable(GL11.GL_BLEND);
        GL11.glEnable(GL11.GL_DEPTH_TEST);
    }
 
}