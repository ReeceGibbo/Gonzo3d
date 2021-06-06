using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gonzo3d.components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Gonzo3d
{
    public static class ShaderHelper
    {
        
        public static void SetInt(ref Shader shader, string name, int data)
        {
            GL.Uniform1(shader.UniformLocations[name], data);
        }

        public static void SetMatrix4(ref Shader shader, string name, Matrix4 data)
        {
            GL.UniformMatrix4(shader.UniformLocations[name], true, ref data);
        }
        
        public static void SetVector3(ref Shader shader, string name, Vector3 data)
        {
            GL.Uniform3(shader.UniformLocations[name], ref data);
        }
        
    }
}