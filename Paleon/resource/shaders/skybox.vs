#version 330
in vec3 position;

out vec3 pass_TextureCoord;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main() {
	gl_Position = projectionMatrix * viewMatrix * vec4(position, 1.0f);
	pass_TextureCoord = position;
}