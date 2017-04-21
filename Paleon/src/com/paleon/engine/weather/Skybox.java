package com.paleon.engine.weather;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL13;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.ResourceManager;
import com.paleon.engine.graph.Mesh;
import com.paleon.engine.graph.ShaderProgram;
import com.paleon.engine.toolbox.GameTime;

public class Skybox {

	private static final float SIZE = 500f;
	
	private static final float[] VERTICES = {        
	    -SIZE,  SIZE, -SIZE,
	    -SIZE, -SIZE, -SIZE,
	    SIZE, -SIZE, -SIZE,
	     SIZE, -SIZE, -SIZE,
	     SIZE,  SIZE, -SIZE,
	    -SIZE,  SIZE, -SIZE,

	    -SIZE, -SIZE,  SIZE,
	    -SIZE, -SIZE, -SIZE,
	    -SIZE,  SIZE, -SIZE,
	    -SIZE,  SIZE, -SIZE,
	    -SIZE,  SIZE,  SIZE,
	    -SIZE, -SIZE,  SIZE,

	     SIZE, -SIZE, -SIZE,
	     SIZE, -SIZE,  SIZE,
	     SIZE,  SIZE,  SIZE,
	     SIZE,  SIZE,  SIZE,
	     SIZE,  SIZE, -SIZE,
	     SIZE, -SIZE, -SIZE,

	    -SIZE, -SIZE,  SIZE,
	    -SIZE,  SIZE,  SIZE,
	     SIZE,  SIZE,  SIZE,
	     SIZE,  SIZE,  SIZE,
	     SIZE, -SIZE,  SIZE,
	    -SIZE, -SIZE,  SIZE,

	    -SIZE,  SIZE, -SIZE,
	     SIZE,  SIZE, -SIZE,
	     SIZE,  SIZE,  SIZE,
	     SIZE,  SIZE,  SIZE,
	    -SIZE,  SIZE,  SIZE,
	    -SIZE,  SIZE, -SIZE,

	    -SIZE, -SIZE, -SIZE,
	    -SIZE, -SIZE,  SIZE,
	     SIZE, -SIZE, -SIZE,
	     SIZE, -SIZE, -SIZE,
	    -SIZE, -SIZE,  SIZE,
	     SIZE, -SIZE,  SIZE
	};
	
	private Mesh mesh;
	private int sunset;
	private int sunny;
	private int night;
	
	private int texture1;
	private int texture2;
	
	private float blendFactor = 0;
	
	private float time = 0;
	
	public Skybox(String firstSkybox, String secondSkybox, String thirdSkybox) {
		mesh = new Mesh(VERTICES, 3);
		sunset = ResourceManager.getSkybox(firstSkybox);
		sunny = ResourceManager.getSkybox(secondSkybox);
		night = ResourceManager.getSkybox(thirdSkybox);
	}
	
	public void update(float deltaTime) {
		time = GameTime.getATime();
		if(time >= 0 && time < 2500){
			texture1 = night;
			texture2 = night;
			blendFactor = (time - 0)/(2500 - 0);
		} else if(time >= 2500 && time < 3500){
			texture1 = night;
			texture2 = sunset;
			blendFactor = (time - 2500)/(3500 - 2500);
		} else if(time >= 3500 && time < 7500){
			texture1 = sunset;
			texture2 = sunny;
			blendFactor = (time - 3500)/(7500 - 3500);
		} else if(time >= 7500 && time < 12000){
			texture1 = sunny;
			texture2 = sunny;
			blendFactor = (time - 7500)/(12000 - 7500);
		} else if(time >= 12000 && time < 16500){
			texture1 = sunny;
			texture2 = sunny;
			blendFactor = (time - 12000)/(16500 - 12000);
		} else if(time >= 16500 && time < 21000){
			texture1 = sunny;
			texture2 = sunset;
			blendFactor = (time - 16500)/(21000 - 16500);
		} else if(time >= 21000 && time < 23500){
			texture1 = sunset;
			texture2 = night;
			blendFactor = (time - 21000)/(23500 - 21000);
		} else {
			texture1 = night;
			texture2 = night;
			blendFactor = (time - 23500)/(24001 - 23500);
		} 
	}
	
	public void render(ShaderProgram shader) {
		GL30.glBindVertexArray(mesh.getVaoId());
		GL20.glEnableVertexAttribArray(0);
		bindTextures(shader);
		GL11.glDrawArrays(GL11.GL_TRIANGLES, 0, mesh.getVertexCount());
		GL20.glDisableVertexAttribArray(0);
		GL30.glBindVertexArray(0);
	}
	
	private void bindTextures(ShaderProgram shader) {
		GL13.glActiveTexture(GL13.GL_TEXTURE0);
		GL11.glBindTexture(GL13.GL_TEXTURE_CUBE_MAP, texture1);
		GL13.glActiveTexture(GL13.GL_TEXTURE1);
		GL11.glBindTexture(GL13.GL_TEXTURE_CUBE_MAP, texture2);
		shader.setUniform("blendFactor", blendFactor);
	}
	
	public void cleanup(){
		mesh.cleanup();
		GL11.glDeleteTextures(sunset);
		GL11.glDeleteTextures(sunny);
	}
}
