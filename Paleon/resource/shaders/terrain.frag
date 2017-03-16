#version 330 core

in vec2 pass_TexCoords;

out vec4 out_Color;

uniform sampler2D atlas;

void main() {
	out_Color = texture(atlas, pass_TexCoords);
}