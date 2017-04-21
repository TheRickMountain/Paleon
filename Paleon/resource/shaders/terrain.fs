#version 330

in vec2 pass_TextureCoord;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;
in float visibility;
in vec3 passFragPos;
in float passHeight;

uniform sampler2D blendMap;
uniform sampler2D aTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;

uniform vec4 lightColor;
uniform vec4 fogColor;

out vec4 out_Color;

vec4 totalColor;

void main() {
	vec4 blendMapColor = texture(blendMap, pass_TextureCoord);
	
	float backTextureAmount = 1 - (blendMapColor.r + blendMapColor.g + blendMapColor.b);
	vec2 tiledCoords = pass_TextureCoord * 80;
	vec4 backgroundTextureColor = texture(aTexture, tiledCoords) * backTextureAmount;
	vec4 rTextureColor = texture(rTexture, tiledCoords) * blendMapColor.r;
	vec4 gTextureColor = texture(gTexture, tiledCoords) * blendMapColor.g;
	vec4 bTextureColor = texture(bTexture, tiledCoords) * blendMapColor.b;
	
	totalColor = backgroundTextureColor + rTextureColor + gTextureColor + bTextureColor;

	vec3 unitNormal = normalize(surfaceNormal);
	vec3 unitLightVector = normalize(toLightVector);

	float brightness = max(dot(unitNormal, unitLightVector), 0.4f);
	vec3 diffuse = brightness * lightColor.rgb;

	out_Color = vec4(diffuse, 1.0f) * totalColor;
	out_Color = mix(vec4(fogColor.rgb, 1.0f), out_Color, visibility);
}