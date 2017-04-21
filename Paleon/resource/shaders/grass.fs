#version 330 core
in vec2 passTextureCoord;
in vec3 toLightVector;
in float visibility;

uniform sampler2D texture_sampler;
uniform vec4 lightColor;
uniform vec4 fogColor;

out vec4 out_Color;

const vec3 normal = vec3(0, 1, 0);

void main() {
	vec4 textureColor = texture(texture_sampler, passTextureCoord);

	if(textureColor.a < 0.5f) {
		discard;
	}

	vec3 unitLightVector = normalize(toLightVector);
	float brightness = max(dot(normal, unitLightVector), 0.4f);
	vec3 diffuse = brightness * lightColor.rgb;

	out_Color = vec4(diffuse, 1.0f) * textureColor;
	out_Color = mix(vec4(fogColor.rgb, 1.0f), out_Color, visibility);
}