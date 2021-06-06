using System.Collections.Generic;

namespace Gonzo3d.components
{
    public struct Shader
    {
        public bool Compiled { get; set; }
        
        public string Name { get; set; }
        public int Handle { get; set; }
        
        public string VertexPath { get; set; }
        public string FragmentPath { get; set; }
        
        public Dictionary<string, int> UniformLocations { get; set; }
    }
}