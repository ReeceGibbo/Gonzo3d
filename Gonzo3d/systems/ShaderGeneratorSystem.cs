using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gonzo3d.components;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;

namespace Gonzo3d.systems
{
    public class ShaderGeneratorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        
        private EcsFilter<Shader> _shaderFilter;

        private Dictionary<string, int> _shaders;
        
        public void Init()
        {
            _shaders = new Dictionary<string, int>();
        }
        
        public void Run()
        {
            foreach (var i in _shaderFilter)
            {
                ref var shader = ref _shaderFilter.Get1(i);

                if (!shader.Compiled)
                {
                    CompileShader(ref shader);
                    _shaders.Add(shader.Name, shader.Handle);
                }
            }
        }

        private void CompileShader(ref Shader shader)
        {
            string vertexShaderSource;

            using (var reader = new StreamReader(shader.VertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;

            using (var reader = new StreamReader(shader.FragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            CompileShader(fragmentShader);

            shader.Handle = GL.CreateProgram();
            GL.AttachShader(shader.Handle, vertexShader);
            GL.AttachShader(shader.Handle, fragmentShader);

            LinkProgram(shader.Handle);
            
            // Cleanup
            GL.DetachShader(shader.Handle, vertexShader);
            GL.DetachShader(shader.Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            
            // Cache Shader Uniform Locations
            GL.UseProgram(shader.Handle);
            GL.GetProgram(shader.Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            shader.UniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(shader.Handle, i, out _, out _);
                var location = GL.GetUniformLocation(shader.Handle, key);

                shader.UniformLocations.Add(key, location);
            }

            shader.Compiled = true;
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
        
    }
}