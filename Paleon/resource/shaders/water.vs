#version 330

layout (location = 0) in vec2 position;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 cameraPosition;
uniform vec3 lightPosition;

out vec4 clipSpace;
out vec2 textureCoords;
out vec3 toCameraVector;
out vec3 fromLightVector;
out vec4 positionRelativeToCamera;

const float tiling = 4.0f;

void main() {
	vec4 mPos = modelMatrix * vec4(position.x, 0, position.y, 1.0f);
	positionRelativeToCamera = viewMatrix * mPos;
	clipSpace = projectionMatrix * positionRelativeToCamera;
	gl_Position = clipSpace;
	textureCoords = vec2(position.x / 2.0f + 0.5f, position.y / 2.0f + 0.5f) * tiling;
	toCameraVector = cameraPosition - mPos.xyz;
	fromLightVector = mPos.xyz - lightPosition;
}