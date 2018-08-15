#version 330 core

uniform sampler2DArray uTxt;
uniform vec3 uColor;
uniform mat3 uUVMat[100];
uniform float uAtlas[100];

in vec2 vUv;
flat in int InstanceID;

out vec4 FragColor;

void main(){
	vec2 uv = (uUVMat[InstanceID] * vec3(vUv, 1.0)).xy;

	vec4 Color = vec4(uColor.xyz, texture(uTxt, vec3(uv, uAtlas[InstanceID])).r);
	if(Color.a <= 0.1)
		discard;
	FragColor = Color;
}