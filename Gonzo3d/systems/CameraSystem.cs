using Gonzo3d.components;
using Leopotam.Ecs;
using OpenTK.Mathematics;

namespace Gonzo3d.systems
{
    public class CameraSystem : IEcsInitSystem, IEcsRunSystem
    {

        private EcsWorld _world;
        private EcsFilter<Camera, Transform> _cameraFilter;

        private float aspectRatio;
        private float fov;
        private float depthNear;
        private float depthFar;

        private int width;
        private int height;

        public CameraSystem(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        
        public void Init()
        {
            aspectRatio = (float) width / (float) height;
            fov = 65f;
            depthNear = 0.01f;
            depthFar = 100.0f;
        }
        
        public void Run()
        {
            foreach (var i in _cameraFilter)
            {
                ref var camera = ref _cameraFilter.Get1(i);
                ref var transform = ref _cameraFilter.Get2(i);

                if (!camera.Init)
                {
                    GenerateCamera(ref camera, ref transform);
                }
                else if (camera.Update)
                {
                    UpdateCamera(ref camera, ref transform);
                }
            }
        }

        private void GenerateCamera(ref Camera camera, ref Transform transform)
        {
            camera.AspectRatio = aspectRatio;
            camera.FieldOfView = fov;

            camera.Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.FieldOfView),
                camera.AspectRatio, depthNear, depthFar);
            
            camera.View = Matrix4.CreateTranslation(transform.Position.X, transform.Position.Y, transform.Position.Z);

            camera.Init = true;
        }

        private void UpdateCamera(ref Camera camera, ref Transform transform)
        {
            camera.View = Matrix4.CreateTranslation(transform.Position.X, transform.Position.Y, transform.Position.Z) *
                          Matrix4.CreateFromQuaternion(new Quaternion(transform.EulerAngles.X, transform.EulerAngles.Y, transform.EulerAngles.Z));
            camera.Update = false;
        }

        
    }
}