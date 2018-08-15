#version 330 core

in vec3 aVertex;
in vec2 aUv;

uniform mat4 uMvp;

out vec2 vUv;

void main(){
	gl_Position = uMvp * vec4(aVertex, 1.0);
	vUv = aUv;
}