using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Gonzo3d
{
    public class Shader
    {
        public int Handle { get; private set; }
        private Dictionary<string, int> UniformLocations;

        public Shader(string name, string vertexPath, string fragmentPath)
        {
            ShaderManager.AddShader(name, this);
            
            UniformLocations = new Dictionary<string, int>();
            
            BuildShader(vertexPath, fragmentPath);
        }
        
        private void BuildShader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource;

            using (var reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;

            using (var reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);
            
            // Cleanup
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            
            // Cache Shader Uniform Locations
            GL.UseProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            UniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);

                UniformLocations.Add(key, location);
            }
        }
        
        private void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }
        
        private void LinkProgram(int program)
        {
            // We link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }
        
        public void SetInt(string name, int data)
        {
            if (UniformLocations.ContainsKey(name))
                GL.Uniform1(UniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            if (UniformLocations.ContainsKey(name))
                GL.UniformMatrix4(UniformLocations[name], true, ref data);
        }
        
        public void SetVector3(string name, Vector3 data)
        {
            if (UniformLocations.ContainsKey(name))
                GL.Uniform3(UniformLocations[name], ref data);
        }
        
    }
}