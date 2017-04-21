#version 330 
layout (location = 0) in vec4 data;

uniform mat4 MP;

out vec2 TextureCoord;

void main(void) {
	gl_Position = MP * vec4(data.xy, 0.0f, 1.0f);
	TextureCoord = data.zw;
}

