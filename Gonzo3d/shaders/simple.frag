#version 460 core

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform vec3 lightDirection;
uniform sampler2D diffuseTexture;

void main()
{
    vec3 color = texture(diffuseTexture, texCoord).rgb;
    vec3 norm = normalize(normal);
    vec3 lightColor = vec3(1.0);

    vec3 ambient = vec3(0.6, 0.6, 0.6) * color;

    vec3 lightDir = normalize(-lightDirection);
    float diff = max(dot(lightDir, norm), 0.0);
    vec3 diffuse = diff * lightColor;

    vec3 result = ambient * diffuse * color;

    outputColor = vec4(result, 1.0);
}