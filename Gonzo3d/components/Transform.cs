using System.Numerics;

namespace Gonzo3d.components
{
    public struct Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}