using System.Numerics;

namespace Gonzo3d.components
{
    public struct Mesh
    {
        public bool Loaded { get; set; }
        public bool Init { get; set; }
        public string RootPath { get; set; }
        public string File { get; set; }
        
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] Uvs { get; set; }
        public uint[] Indices { get; set; }
        
        // OpenGL
        public int Vao { get; set; }
        public int Vbo { get; set; }
        public int Ebo { get; set; }
    }
}