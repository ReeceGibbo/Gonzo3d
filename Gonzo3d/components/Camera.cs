using OpenTK.Mathematics;

namespace Gonzo3d.components
{
    public struct Camera
    {
        public bool Init { get; set; }
        public bool Update { get; set; }
        
        public float FieldOfView { get; set; }
        
        public float AspectRatio { get; set; }
        
        public Matrix4 View { get; set; }
        public Matrix4 Projection { get; set; }
    }
}