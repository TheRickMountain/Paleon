package com.paleon.engine.graph;

import static org.lwjgl.opengl.GL20.GL_COMPILE_STATUS;
import static org.lwjgl.opengl.GL20.GL_FRAGMENT_SHADER;
import static org.lwjgl.opengl.GL20.GL_LINK_STATUS;
import static org.lwjgl.opengl.GL20.GL_VALIDATE_STATUS;
import static org.lwjgl.opengl.GL20.GL_VERTEX_SHADER;
import static org.lwjgl.opengl.GL20.glAttachShader;
import static org.lwjgl.opengl.GL20.glCompileShader;
import static org.lwjgl.opengl.GL20.glCreateProgram;
import static org.lwjgl.opengl.GL20.glCreateShader;
import static org.lwjgl.opengl.GL20.glDeleteProgram;
import static org.lwjgl.opengl.GL20.glDetachShader;
import static org.lwjgl.opengl.GL20.glGetProgrami;
import static org.lwjgl.opengl.GL20.glGetShaderInfoLog;
import static org.lwjgl.opengl.GL20.glGetShaderi;
import static org.lwjgl.opengl.GL20.glGetUniformLocation;
import static org.lwjgl.opengl.GL20.glLinkProgram;
import static org.lwjgl.opengl.GL20.glShaderSource;
import static org.lwjgl.opengl.GL20.glUniform1f;
import static org.lwjgl.opengl.GL20.glUniform1i;
import static org.lwjgl.opengl.GL20.glUniform2f;
import static org.lwjgl.opengl.GL20.glUniform3f;
import static org.lwjgl.opengl.GL20.glUniform4f;
import static org.lwjgl.opengl.GL20.glUniformMatrix4fv;
import static org.lwjgl.opengl.GL20.glUseProgram;
import static org.lwjgl.opengl.GL20.glValidateProgram;

import java.io.InputStream;
import java.nio.FloatBuffer;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;

import org.lwjgl.BufferUtils;

import com.paleon.engine.toolbox.Color;
import com.paleon.maths.vecmath.Matrix4f;
import com.paleon.maths.vecmath.Vector2f;
import com.paleon.maths.vecmath.Vector3f;
import com.paleon.maths.vecmath.Vector4f;


public class ShaderProgram {

	private final int programId;

    private int vertexShaderId;

    private int fragmentShaderId;

    private int geometryShaderId;

    private final Map<String, UniformData> uniforms;

    public ShaderProgram() throws Exception {
        programId = glCreateProgram();
        if (programId == 0) {
            throw new Exception("Could not create Shader");
        }
        uniforms = new HashMap<>();
    }
    
    public void createUniform(String uniformName) {
        int uniformLocation = glGetUniformLocation(programId, uniformName);
        if (uniformLocation < 0) {
        	System.err.println("Could not find uniform: " + uniformName);
        	return;
        }
        uniforms.put(uniformName, new UniformData(uniformLocation));
    }
    
    public void setUniform(String uniformName, boolean value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
            System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
	        int temp = 0;
	        if(value){
	        	temp = 1;
	        }
	        glUniform1i(uniformData.getUniformLocation(), temp);
        }
    }
    
    public void setUniform(String uniformName, boolean value, boolean bindShader) {
    	if(bindShader)
    		bind();
        setUniform(uniformName, value);
        if(bindShader)
        	unbind();
    }
    
    public void setUniform(String uniformName, int value) {
    	UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
            System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform1i(uniformData.getUniformLocation(), value);
        }
    }
    
    public void setUniform(String uniformName, int value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }

    public void setUniform(String uniformName, float value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform1f(uniformData.getUniformLocation(), value);
        }
    }
    
    public void setUniform(String uniformName, float value, boolean bindShader) {
    	if(bindShader)
    		bind();
        setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }

    public void setUniform(String uniformName, float x, float y) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform2f(uniformData.getUniformLocation(), x, y);
        }
    }
    
    public void setUniform(String uniformName, float x, float y, boolean bindShader) {
    	if(bindShader)
    		bind();
        setUniform(uniformName, x, y);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, Vector2f value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform2f(uniformData.getUniformLocation(), value.x, value.y);
        }
    }
    
    public void setUniform(String uniformName, Vector2f value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, float x, float y, float z) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform3f(uniformData.getUniformLocation(), x, y, z);
        }
    }
    
    public void setUniform(String uniformName, float x, float y, float z, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, x, y, z);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, Vector3f value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
           System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform3f(uniformData.getUniformLocation(), value.x, value.y, value.z);
        }
    }
    
    public void setUniform(String uniformName, Vector3f value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, float x, float y, float z, float w) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform4f(uniformData.getUniformLocation(), x, y, z, w);
        }
    }
    
    public void setUniform(String uniformName, float x, float y, float z, float w, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, x, y, z, w);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, Vector4f value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform4f(uniformData.getUniformLocation(), value.x, value.y, value.z, value.w);
        }
    }
    
    public void setUniform(String uniformName, Vector4f value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, Color value) {
        UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
        	glUniform4f(uniformData.getUniformLocation(), value.getRf(), value.getGf(), value.getBf(), value.getAf());
        }
    }
    
    public void setUniform(String uniformName, Color value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }
    
    public void setUniform(String uniformName, Matrix4f value) {
    	UniformData uniformData = uniforms.get(uniformName);
        if (uniformData == null) {
        	System.err.println("Uniform [" + uniformName + "] has not been created");
        } else {
	        FloatBuffer fb = uniformData.getFloatBuffer();
	        if(fb == null) {
	        	fb = BufferUtils.createFloatBuffer(16);
	        	uniformData.setFloatBuffer(fb);
	        }
	        value.store(fb);
	        fb.flip();
	        glUniformMatrix4fv(uniformData.getUniformLocation(), false, fb);
        }
    }
    
    public void setUniform(String uniformName, Matrix4f value, boolean bindShader) {
    	if(bindShader)
    		bind();
    	setUniform(uniformName, value);
        if(bindShader)
    		unbind();
    }
    
    public void createVertexShader(String shaderFile) throws Exception {
        vertexShaderId = createShader(loadResource(shaderFile), GL_VERTEX_SHADER);
    }

    public void createFragmentShader(String shaderFile) throws Exception {	
        fragmentShaderId = createShader(loadResource(shaderFile), GL_FRAGMENT_SHADER);
    }

    protected int createShader(String shaderCode, int shaderType) throws Exception {
        int shaderId = glCreateShader(shaderType);
        if (shaderId == 0) {
            throw new Exception("Error creating shader. Code: " + shaderId);
        }

        glShaderSource(shaderId, shaderCode);
        glCompileShader(shaderId);

        if (glGetShaderi(shaderId, GL_COMPILE_STATUS) == 0) {
        	String type = "";
        	if(shaderType == GL_VERTEX_SHADER) {
        		type = "Vertex Shader";
        	} else if(shaderType == GL_FRAGMENT_SHADER) {
        		type = "Fragment Shader";
        	}
        	
            throw new Exception("Error compiling " + type + " code: " + glGetShaderInfoLog(shaderId, 1024));
        }

        glAttachShader(programId, shaderId);

        return shaderId;
    }

    public void link() throws Exception {
        glLinkProgram(programId);
        if (glGetProgrami(programId, GL_LINK_STATUS) == 0) {
            throw new Exception("Error linking Shader code: " + glGetShaderInfoLog(programId, 1024));
        }

        glValidateProgram(programId);
        if (glGetProgrami(programId, GL_VALIDATE_STATUS) == 0) {
            System.err.println("Warning validating Shader code: " + glGetShaderInfoLog(programId, 1024));
        }

    }

    public void bind() {
        glUseProgram(programId);
    }

    public void unbind() {
        glUseProgram(0);
    }

    public void cleanup() {
        unbind();
        if (programId != 0) {
            if (vertexShaderId != 0) {
                glDetachShader(programId, vertexShaderId);
            }
            if ( geometryShaderId != 0) {
                glDetachShader(programId, geometryShaderId);
            }
            if (fragmentShaderId != 0) {
                glDetachShader(programId, fragmentShaderId);
            }
            glDeleteProgram(programId);
        }
    }
    
    @SuppressWarnings("resource")
	private static String loadResource(String fileName) throws Exception {
        String result = "";
        try (InputStream in = ShaderProgram.class.getClass().getResourceAsStream(fileName)) {
            result = new Scanner(in, "UTF-8").useDelimiter("\\A").next();
        }
        return result;
    }
}
