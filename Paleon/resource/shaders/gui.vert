#version 330 core

in vec2 in_data;

out vec2 pass_TexCoords;

uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;

void main() {
	
	gl_Position = projectionMatrix * modelMatrix * vec4(in_data, 0.0f, 1.0f);
	pass_TexCoords = in_data;

}