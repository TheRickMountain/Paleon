#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 textureCoord;
layout (location = 2) in vec3 offset;

uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform vec3 lightPosition;

out vec2 passTextureCoord;
out vec3 toLightVector;
out float visibility;

const float density = 0.0035f;
const float gradient = 5.0f;

void main() {
	vec4 mPos = modelMatrix * vec4(position + offset, 1.0f);
	vec4 mvPos = viewMatrix * mPos;
	gl_Position = projectionMatrix * mvPos;
	passTextureCoord = textureCoord;
	toLightVector = lightPosition - mPos.xyz;

	float distance = length(mvPos.xyz);
	visibility = exp(-pow((distance * density), gradient));
	visibility = clamp(visibility, 0.0f, 1.0f);
}