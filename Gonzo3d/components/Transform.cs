
using OpenTK.Mathematics;

namespace Gonzo3d.components
{
    public struct Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 EulerAngles { get; set; }
        public Vector3 Scale { get; set; }
    }
}