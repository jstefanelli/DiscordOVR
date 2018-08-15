#version 330 core

in vec3 aVertex;
in vec2 aUv;

uniform mat4 uMvp;
uniform mat4 uOffset[100];

out vec2 vUv;
flat out int InstanceID;

void main(){
	vUv = vec2(aUv.x, 1 - aUv.y);
	gl_Position = uMvp * uOffset[gl_InstanceID] * vec4(aVertex, 1.0);
	InstanceID = gl_InstanceID;
}