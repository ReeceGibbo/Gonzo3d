#version 460 core

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform sampler2D mainTex;

uniform vec3 lightPos;

void main()
{
    vec3 lightColor = vec3(1.0, 1.0, 1.0);
    vec3 objectColor = vec3(1.0, 1.0, 1.0);
    
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;
    
    // Lighting
    vec3 norm = normalize(normal);
    vec3 lightDirection = normalize(lightPos - fragPos);
    
    float difference = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = difference * lightColor;
    
    vec3 result = (ambient + diffuse) * objectColor;
    
    outputColor = texture(mainTex, texCoord) * vec4(result, 1.0);
}