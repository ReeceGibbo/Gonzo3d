using System.Numerics;

namespace Gonzo3d.components
{
    public struct Mesh
    {
        public bool Loaded { get; set; }
        public bool Init { get; set; }
        public string Path { get; set; }
        
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] Uvs { get; set; }
        public uint[] Indices { get; set; }
        
        // OpenGL
        private int Vao { get; set; }
        private int Vbo { get; set; }
        private int Ebo { get; set; }
    }
}