using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Gonzo3d
{
    public class Shader : IDisposable
    {
        public int handle;

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
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void Dispose()
        {
            GL.DeleteProgram(handle);
        }
    }
}