#version 330

in vec2 passTextureCoords;

uniform vec4 color;
uniform sampler2D fontAtlas;

out vec4 fragColor;

void main(void){

	fragColor = vec4(color.rgb, texture(fontAtlas, passTextureCoords).a);

}