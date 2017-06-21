#version 330 core

in vec2 TextureCoords;

out vec4 color;

uniform sampler2D diffuse;

void main(void) 
{
	color = texture(diffuse, TextureCoords);
}