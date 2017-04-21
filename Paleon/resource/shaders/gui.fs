#version 330

in vec2 TextureCoord;

uniform sampler2D image;
uniform int mode;
uniform vec4 spriteColor;

out vec4 out_Color;

void main(void) {
	
	if(mode == 1) {
		out_Color = spriteColor;
	} else if(mode == 2) {
		out_Color = vec4(spriteColor.rgb, texture(image, TextureCoord).a);
	} else {
		out_Color = spriteColor * texture(image, TextureCoord);
	}
}