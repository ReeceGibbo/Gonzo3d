#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormals;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 lightSpaceMatrix;

out vec2 texCoord;
out vec3 normal;
out vec3 fragPos;
out vec4 fragPosLightSpace;

void main()
{
    fragPos = vec3(vec4(aPosition, 1.0) * model);
    normal = aNormals * mat3(transpose(inverse(model)));
    texCoord = aTexCoord;
    fragPosLightSpace = vec4(fragPos, 1.0) * lightSpaceMatrix;
    
    gl_Position = vec4(fragPos, 1.0) * view * projection;
}