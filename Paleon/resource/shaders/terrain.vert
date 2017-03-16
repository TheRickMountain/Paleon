#version 330 core

in vec2 in_position;
in vec2 in_texCoords;

out vec2 pass_TexCoords;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	
	gl_Position = projectionMatrix * viewMatrix * vec4(in_position, 0.0f, 1.0f);
	pass_TexCoords = in_texCoords;

}