#version 330 core

in vec3 aVertex;

uniform mat4 uMvp;

void main(){
	gl_Position = uMvp * vec4(aVertex, 1.0);
}