using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Gonzo3d
{
    public class Shader : IDisposable
    {
        public int handle;
        
        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource;

            using (var reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (var reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, VertexShaderSource);
            
            var infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (infoLogVert != string.Empty)
                Debug.WriteLine(infoLogVert);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, FragmentShaderSource);
            
            var infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
            if (infoLogFrag != string.Empty)
                Debug.WriteLine(infoLogFrag);

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);
            
            // Cleanup
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            
            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data);
        }
        
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void Dispose()
        {
            GL.DeleteProgram(handle);
        }
    }
}