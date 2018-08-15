#version 330 core

uniform sampler2D uTxt;

in vec2 vUv;

void main(){
	gl_FragColor = texture2D(uTxt, vUv);
}