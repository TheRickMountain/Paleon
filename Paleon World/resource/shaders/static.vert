#version 330 core

in vec3 position;
in vec2 textureCoords;
in vec3 normal;

out vec2 TextureCoords;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

void main(void) 
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0f);
	TextureCoords = textureCoords;
}