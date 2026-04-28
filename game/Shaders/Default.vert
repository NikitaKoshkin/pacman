#version 330 core
layout (location = 0) in vec3 aPosition; // vertex coordinates
layout (location = 1) in vec2 aTexCoord; // texture coordinates
layout (location = 2) in vec3 aNormal;


out vec2 texCoord;
out vec3 Normal;
out vec3 FragPos;

// uniform variables
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection; //coordinates
	FragPos = vec3(vec4(aPosition, 1.0) * model);
	texCoord = aTexCoord;
	Normal = aNormal * mat3(transpose(inverse(model)));
}