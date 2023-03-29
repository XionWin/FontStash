#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 color;

uniform sampler2D texture0;

void main()
{
	outputColor = vec4(color.x, color.y, color.z, texture(texture0, texCoord).x * color.w);
}