#version 450

out vec4 outputColor;

in vec2 texCoord;

uniform vec4 color;

void main()
{
    outputColor = color;
}