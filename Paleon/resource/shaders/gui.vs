#version 330 
layout (location = 0) in vec4 data;

uniform mat4 MP;
uniform int mode;

out vec2 TextureCoord;

const vec2 offset = vec2(2.0f, -2.0f);

void main(void) {
	if(mode == 2) {
		gl_Position = MP * vec4(data.xy, 0.0f, 1.0f);
	} else {
		gl_Position = MP * vec4(data.xy, 0.0f, 1.0f);
	}
	TextureCoord = data.zw;
}

