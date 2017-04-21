#version 330

in vec4 clipSpace;
in vec2 textureCoords;
in vec3 toCameraVector;
in vec3 fromLightVector;
in vec4 positionRelativeToCamera;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D dudvMap;
uniform sampler2D normalMap;
uniform sampler2D depthMap;
uniform vec4 lightColor;
uniform vec4 fogColor;

uniform float moveFactor;

out vec4 out_Color;

const float waveStrength = 0.04f;
const float shineDamper = 20.0f;
const float reflectivity = 0.5f;
const float density = 0.0035f;
const float gradient = 5.0f;


void main() {
	vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0f + 0.5f;
	vec2 refractTexCoords = vec2(ndc.x, ndc.y);
	vec2 reflectTexCoords = vec2(ndc.x, -ndc.y);

	float near = 0.1f;
	float far = 1000.0f;
	float depth = texture(depthMap, refractTexCoords).r;
	float floorDistance = 2.0f * near * far / (far + near - (2.0f * depth - 1.0f) * (far - near));

	depth = gl_FragCoord.z;
	float waterDistance = 2.0f * near * far / (far + near - (2.0f * depth - 1.0f) * (far - near));
	float waterDepth = floorDistance - waterDistance;

	vec2 distortedTexCoords = texture(dudvMap, vec2(textureCoords.x + moveFactor, textureCoords.y)).rg*0.1;
	distortedTexCoords = textureCoords + vec2(distortedTexCoords.x, distortedTexCoords.y+moveFactor);
	vec2 totalDistortion = (texture(dudvMap, distortedTexCoords).rg * 2.0 - 1.0) * waveStrength * clamp(waterDepth / 20.0f, 0.0f, 1.0f);

	refractTexCoords += totalDistortion;
	refractTexCoords = clamp(refractTexCoords, 0.001f, 0.999f);

	reflectTexCoords += totalDistortion;
	reflectTexCoords.x = clamp(reflectTexCoords.x, 0.001f, 0.999f);
	reflectTexCoords.y = clamp(reflectTexCoords.y, -0.999f, -0.001f);

	vec4 reflectColor = texture(reflectionTexture, reflectTexCoords);
	vec4 refractColor = texture(refractionTexture, refractTexCoords);

	vec4 normalMapColor = texture(normalMap, distortedTexCoords);
	vec3 normal = vec3(normalMapColor.r * 2.0f - 1.0f, normalMapColor.b * 3.0f, normalMapColor.g * 2.0f - 1.0f);
	normal = normalize(normal);

	vec3 viewVector = normalize(toCameraVector);
	float refractiveFactor = dot(viewVector, normal);
	refractiveFactor = pow(refractiveFactor, 0.5f);
	refractiveFactor = clamp(refractiveFactor, 0.0f, 1.0f);

	vec3 reflectedLight = reflect(normalize(fromLightVector), normal);
	float specular = max(dot(reflectedLight, viewVector), 0.0);
	specular = pow(specular, shineDamper);
	vec3 specularHighlights = lightColor.rgb * specular * reflectivity * clamp(waterDepth / 5.0f, 0.0f, 1.0f);;

	float distance = length(positionRelativeToCamera.xyz);
	float visibility = exp(-pow((distance * density), gradient));
	visibility = clamp(visibility, 0.0f, 1.0f);

	out_Color = mix(reflectColor, refractColor, refractiveFactor);
	out_Color = mix(out_Color, vec4(0.2f, 0.45f, 0.4f, 1.0f), 0.2f) + vec4(specularHighlights, 0.0f);
	out_Color.a = clamp(waterDepth / 5.0f, 0.0f, 1.0f);
	out_Color = mix(vec4(fogColor.rgb, 1.0f), out_Color, visibility);
}