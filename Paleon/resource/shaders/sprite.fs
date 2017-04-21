#version 330

in vec2 TextureCoord;

uniform sampler2D image;
uniform int solidColor;
uniform vec4 spriteColor;

out vec4 out_Color;

void main(void) {
	
	if(solidColor == 1) {
		out_Color = spriteColor;
	} else {
		out_Color = spriteColor * texture(image, TextureCoord);
	}
}