#version 330

in vec3 pass_TextureCoord;

uniform samplerCube sampler_cube_1;
uniform samplerCube sampler_cube_2;
uniform float blendFactor;
uniform vec4 fogColor;

out vec4 out_Color;

const float lowerLimit = 0.0f;
const float upperLimit = 220.0f;

void main() {
	vec4 cubemap_1 = texture(sampler_cube_1, pass_TextureCoord);
	vec4 cubemap_2 = texture(sampler_cube_2, pass_TextureCoord);
	vec4 finalColor = mix(cubemap_1, cubemap_2, blendFactor);

	float factor = (pass_TextureCoord.y - lowerLimit) / (upperLimit - lowerLimit);
	factor = clamp(factor, 0.0f, 1.0f);
	out_Color = mix(vec4(fogColor.rgb, 1.0f), finalColor, factor);
}