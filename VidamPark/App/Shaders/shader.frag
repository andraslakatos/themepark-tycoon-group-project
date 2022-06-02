#version 450

out vec4 outputColor;

in vec2 texCoord;

uniform vec4 color;

uniform int isColor;
uniform sampler2D texture0;

void main()
{
    if(isColor==1)
    {
        outputColor = color;
    }
    else if(isColor == 0)
    {
        outputColor = texture(texture0, texCoord);
    }
    else if(isColor == 2)
    {
        outputColor = texture(texture0, texCoord) * color;
    }
}