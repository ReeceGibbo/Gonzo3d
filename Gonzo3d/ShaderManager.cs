using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Gonzo3d
{
    public class ShaderManager
    {
        private static Dictionary<string, Shader> shaders;

        public ShaderManager()
        {
            shaders = new Dictionary<string, Shader>();
        }

        public static void AddShader(string name, Shader shader)
        {
            shaders.Add(name, shader);
        }

        public static Shader GetShader(string shader)
        {
            return shaders.ContainsKey(shader) ? shaders[shader] : null;
        }

    }
}