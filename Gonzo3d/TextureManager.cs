using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gonzo3d
{
    public class TextureManager
    {
        private static Dictionary<string, Texture> textures;

        public TextureManager()
        {
            textures = new Dictionary<string, Texture>();
        }

        public static void AddTexture(string name, Texture texture)
        {
            textures.Add(name, texture);
        }

        public static Texture GetTexture(string texture)
        {
            return textures.ContainsKey(texture) ? textures[texture] : null;
        }

        public static void CreateTexture(string name, string path)
        {
            var texture = new Texture(name, path);
        }
    }
}