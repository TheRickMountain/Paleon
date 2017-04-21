#version 330
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 textureCoord;
layout (location = 2) in vec3 normal;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 lightPosition;
uniform vec4 plane;

out vec3 surfaceNormal;
out vec3 toLightVector;
out vec3 toCameraVector;
out vec2 pass_TextureCoord;
out float visibility;
out vec3 passFragPos;
out float passHeight;

const float density = 0.0035f;
const float gradient = 5.0f;
const float shadowDistance = 150.0f;
const float transitionDistance = 10.0f;

void main(){
	passHeight = position.y;
	vec4 mPos = modelMatrix * vec4(position, 1.0f);

	passFragPos = vec3(mPos);

	gl_ClipDistance[0] = dot(mPos, plane);

	vec4 mvPos = viewMatrix * mPos;
	gl_Position = projectionMatrix * mvPos;

	pass_TextureCoord = textureCoord;

	surfaceNormal = (modelMatrix * vec4(normal, 0.0f)).xyz;
	toLightVector = lightPosition - mPos.xyz;
	toCameraVector = (inverse(viewMatrix) * vec4(0.0f, 0.0f, 0.0f, 1.0f)).xyz - mPos.xyz;

	float distance = length(mvPos.xyz);
	visibility = exp(-pow((distance * density), gradient));
	visibility = clamp(visibility, 0.0f, 1.0f);

	distance = distance - (shadowDistance - transitionDistance);
	distance = distance / transitionDistance;
}