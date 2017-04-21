package com.paleon.engine.graph;

import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GL15;
import org.lwjgl.opengl.GL20;
import org.lwjgl.opengl.GL30;

import com.paleon.engine.toolbox.OpenglUtils;

public class Mesh {

	private int vaoId;
	
	private int vVboId;
	private int uvVboId;
	private int nVboId;
	private int tVboId;
	
	private int vertexCount;
	
	private Material material;
	
	private float[] vertices;
	private float[] uvs;
	private float[] normals;
	private int[] triangles;
	
	private float furthestPoint = 0;
	
	public Mesh(float[] vertices, float[] uvs, float[] normals, int[] triangles) {
		this.vertices = vertices;
		this.uvs = uvs;
		this.normals = normals;
		this.triangles = triangles;
		
		this.vertexCount = triangles.length;
		 
		vaoId = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vaoId);
		
		vVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(vertices), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(0, 3, GL11.GL_FLOAT, false, 0, 0);
		
		uvVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, uvVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(uvs), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, 0, 0);
		
		nVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, nVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(normals), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(2, 3, GL11.GL_FLOAT, false, 0, 0);
		
		tVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, tVboId);
		GL15.glBufferData(GL15.GL_ELEMENT_ARRAY_BUFFER, OpenglUtils.dataToIntBuffer(triangles), GL15.GL_STATIC_DRAW);
		
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
		
		furthestPoint = getFurthestPoint(vertices);
	}
	
	public Mesh(float[] data, int subMeshCount){
		this.vertices = data;
		
		this.vertexCount = data.length / subMeshCount;
		
		vaoId = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vaoId);
		
		vVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(data), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(0, subMeshCount, GL11.GL_FLOAT, false, 0, 0);
		
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
		
		//furthestPoint = getFurthestPoint(data);
	}
	
	public Mesh(float[] vertices, float[] uvs) {
		this.vertices = vertices;
		this.uvs = uvs;
		
		this.vertexCount = vertices.length;
		
		vaoId = GL30.glGenVertexArrays();
		GL30.glBindVertexArray(vaoId);
		
		vVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(vertices), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(0, 2, GL11.GL_FLOAT, false, 0, 0);
		
		uvVboId = GL15.glGenBuffers();
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, uvVboId);
		GL15.glBufferData(GL15.GL_ARRAY_BUFFER, OpenglUtils.dataToFloatBuffer(uvs), GL15.GL_STATIC_DRAW);
		GL20.glVertexAttribPointer(1, 2, GL11.GL_FLOAT, false, 0, 0);
		
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		GL30.glBindVertexArray(0);
		
		furthestPoint = getFurthestPoint(vertices);
	}
	
	public void setVertices(float[] vertices) {
		this.vertices = vertices;
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, vVboId);
		GL15.glBufferSubData(GL15.GL_ARRAY_BUFFER, 0, OpenglUtils.dataToFloatBuffer(vertices));
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
		
		furthestPoint = getFurthestPoint(vertices);
	}
	
	public void setUVs(float[] uvs) {
		this.uvs = uvs;
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, uvVboId);
		GL15.glBufferSubData(GL15.GL_ARRAY_BUFFER, 0, OpenglUtils.dataToFloatBuffer(uvs));
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
	}
	
	public void setNormals(float[] normals) {
		this.normals = normals;
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, nVboId);
		GL15.glBufferSubData(GL15.GL_ARRAY_BUFFER, 0, OpenglUtils.dataToFloatBuffer(normals));
		GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
	}
	
	public void setTriangles(int[] triangles) {
		this.triangles = triangles;
		this.vertexCount = triangles.length;
		GL15.glBindBuffer(GL15.GL_ELEMENT_ARRAY_BUFFER, tVboId);
		GL15.glBufferSubData(GL15.GL_ELEMENT_ARRAY_BUFFER, 0, OpenglUtils.dataToIntBuffer(triangles));
	}
	
	public int getVaoId() {
        return vaoId;
    }

    public int getVertexCount() {
        return vertexCount;
    }

    public void setVertexCount(int vertexCount) {
		this.vertexCount = vertexCount;
	}

	public Material getMaterial() {
		return material;
	}

	public void setMaterial(Material material) {
		this.material = material;
	}

	public float[] getVertices() {
		return vertices;
	}

	public float[] getUVs() {
		return uvs;
	}

	public float[] getNormals() {
		return normals;
	}

	public int[] getTriangles() {
		return triangles;
	}
	
	public float getFurthestPoint() {
		return furthestPoint;
	}
	
	private float getFurthestPoint(float[] vertices) {
		float maxX = 0;
		float maxY = 0;
		float maxZ = 0;
		
		float x = 0;
		float y = 0;
		float z = 0;
		
		int temp = 0;
		for(int i = 0; i < vertices.length; i += 3) {
			temp = i;
			x = vertices[temp];
			temp++;
			y = vertices[temp];
			temp++;
			z = vertices[temp];
			
			if(x < 0) {
				x = -x;
			}
			
			if(y < 0) {
				y = -y;
			}
			
			if(z < 0) {
				z = -z;
			}
			
			if(maxX < x) {
				maxX = x;
			}
			
			if(maxY < y) {
				maxY = y;
			}
			
			if(maxZ < z) {
				maxZ = z;
			}
		}
		return Math.max(maxX, maxZ);
	}

	public void cleanup() {
        GL20.glDisableVertexAttribArray(0);

        GL15.glBindBuffer(GL15.GL_ARRAY_BUFFER, 0);
        GL15.glDeleteBuffers(vVboId);
        GL15.glDeleteBuffers(uvVboId);
        GL15.glDeleteBuffers(nVboId);
        GL15.glDeleteBuffers(tVboId);

        GL30.glBindVertexArray(0);
        GL30.glDeleteVertexArrays(vaoId);
    }
}
