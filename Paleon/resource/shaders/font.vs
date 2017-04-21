#version 330

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 textureCoords;

out vec2 passTextureCoords;

uniform vec2 translation;

void main(void){

	gl_Position = vec4(position + translation * vec2(2.0, -2.0), 0.0, 1.0);
	passTextureCoords = textureCoords;

}