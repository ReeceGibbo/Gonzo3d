using Gonzo3d.components;
using Leopotam.Ecs;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gonzo3d.systems
{
    public class CameraMovementSystem : IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter<Camera, Transform> _cameraFilter;
        
        public void Run()
        {
            var keyboard = InputManager.KeyboardState;
            var mouse = InputManager.MouseState;
            var speed = 0.001f;
            
            foreach (var i in _cameraFilter)
            {
                ref var camera = ref _cameraFilter.Get1(i);
                ref var transform = ref _cameraFilter.Get2(i);

                // Position Movement
                var position = transform.Position;

                var quaternion = new Quaternion(-transform.EulerAngles.X, -transform.EulerAngles.Y,
                    transform.EulerAngles.Z);

                var movement = new Vector3(0, 0, 0);
                
                if (keyboard.IsKeyDown(Keys.W))
                {
                    movement.Z = 1;
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    movement.Z = -1;
                }
                if (keyboard.IsKeyDown(Keys.A))
                {
                    movement.X = 1;
                }
                if (keyboard.IsKeyDown(Keys.D))
                {
                    movement.X = -1;
                }

                transform.Position = position + (quaternion * movement * speed);

                // Rotation Movement
                var eulerAngles = transform.EulerAngles;
                eulerAngles.X += speed * (mouse.Y - mouse.PreviousY);
                eulerAngles.Y += speed * (mouse.X - mouse.PreviousX);
                eulerAngles.Z = 0f;

                transform.EulerAngles = eulerAngles;
                camera.Update = true;
            }
        }
    }
}