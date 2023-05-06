#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 color;

uniform sampler2D texture0;

void main()
{
	outputColor = vec4(color.xyz, texture(texture0, texCoord).w * color.w);
}