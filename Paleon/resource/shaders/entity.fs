#version 330

in vec2 passTextureCoord;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;
in float visibility;

uniform sampler2D texture_sampler;
uniform vec4 lightColor;
uniform float shineDamper;
uniform float reflectivity;
uniform vec4 objectColor;
uniform int colorMode;
uniform int useFakeLighting;
uniform vec4 fogColor;

out vec4 out_Color;
vec4 textureColor;

void main() {
	if(colorMode == 1) {
		out_Color = objectColor;
	} else {
		textureColor = texture(texture_sampler, passTextureCoord);

		if(textureColor.a < 0.5f) {
			discard;
		}

		vec3 unitNormal = normalize(surfaceNormal);
		if(useFakeLighting == 1) {
			unitNormal = vec3(0, 1, 0);
		}
		vec3 unitLightVector = normalize(toLightVector);

		float brightness = max(dot(unitNormal, unitLightVector), 0.4f);
		vec3 diffuse = brightness * lightColor.rgb;

		vec3 unitVectorToCamera = normalize(toCameraVector);
		vec3 lightDirection = -unitLightVector;
		vec3 reflectedLightDirection = reflect(lightDirection, unitNormal);

		float specularFactor = dot(reflectedLightDirection, unitVectorToCamera);
		specularFactor = max(specularFactor, 0.0f);
		float dampedFactor = pow(specularFactor, shineDamper);
		vec3 finalSpecular = dampedFactor * reflectivity * lightColor.rgb;

		out_Color = vec4(diffuse, 1.0f) * textureColor + vec4(finalSpecular, 1.0f);
		out_Color = mix(vec4(fogColor.rgb, 1.0f), out_Color, visibility);
	}
}