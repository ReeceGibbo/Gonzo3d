namespace Gonzo3d.components
{
    public struct Material
    {
        public bool Init { get; set; }
        public string ShaderToUse { get; set; }
        
        public int VertexLocation { get; set; }
        public int TexCoordLocation { get; set; }
        public int NormalLocation { get; set; }
    }
}