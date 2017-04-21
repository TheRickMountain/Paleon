#version 330
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 textureCoord;
layout (location = 2) in vec3 normal;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 lightPosition;
uniform int useWaving;
uniform float wavingValue;
uniform int numberOfRows;
uniform vec2 offset;
uniform vec4 plane;

out vec3 surfaceNormal;
out vec3 toLightVector;
out vec3 toCameraVector;
out vec2 passTextureCoord;
out float visibility;

const float density = 0.0035f;
const float gradient = 5.0f;

void main(){
	vec4 pos = vec4(position, 1.0f);

	if(useWaving == 1) {
		if(pos.y > 0.5f) {
			pos.x += wavingValue;
		}
	}

	vec4 mPos = modelMatrix * pos;

	gl_ClipDistance[0] = dot(mPos, plane);

	vec4 mvPos = viewMatrix * mPos;
	gl_Position = projectionMatrix * mvPos;

	passTextureCoord = (textureCoord / numberOfRows) + offset;

	surfaceNormal = (modelMatrix * vec4(normal, 0.0f)).xyz;
	toLightVector = lightPosition - mPos.xyz;
	toCameraVector = (inverse(viewMatrix) * vec4(0.0f, 0.0f, 0.0f, 1.0f)).xyz - mPos.xyz;

	float distance = length(mvPos.xyz);
	visibility = exp(-pow((distance * density), gradient));
	visibility = clamp(visibility, 0.0f, 1.0f);

}