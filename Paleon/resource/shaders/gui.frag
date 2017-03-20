#version 330 core

in vec2 pass_TexCoords;

out vec4 out_Color;

uniform sampler2D sprite;
uniform vec4 color;
uniform int hasTexture;

void main() {
	if(hasTexture == 1) {
		out_Color = texture(sprite, pass_TexCoords) * color;
	} else {
		out_Color = color;
	}
}