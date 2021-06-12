using System.IO;
using System.Reflection;
using Gonzo3d.components;
using Gonzo3d.systems;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Camera = Gonzo3d.components.Camera;
using Material = Gonzo3d.components.Material;
using Mesh = Gonzo3d.components.Mesh;

namespace Gonzo3d
{
    public class Game : GameWindow
    {
        private ShaderManager _shaderManager;
        private TextureManager _textureManager;
        
        private EcsWorld _world;
        private EcsSystems _systems;

        //private Vector3 lightDirection = new Vector3(0.5f, -0.8f, -0.8f);

        private int width;
        private int height;
        
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            width = nativeWindowSettings.Size.X;
            height = nativeWindowSettings.Size.Y;

            CursorGrabbed = true;
            Cursor = MouseCursor.Empty;
        }
        
        protected override void OnLoad()
        {
            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            
            // Load our Shader manager and Shaders
            _shaderManager = new ShaderManager();
            var mainShader = new Shader("MainShader", "shaders/basic.vert", "shaders/basic.frag");
            mainShader.SetInt("diffuseTexture", 0);
            mainShader.SetInt("shadowMap", 1);

            var shadowShader = new Shader("ShadowShader", "shaders/shadow.vert", "shaders/shadow.frag");

            _textureManager = new TextureManager();
            
            var shadowMap = new ShadowMap();

            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems.Add(new MeshGeneratorSystem());
            _systems.Add(new MaterialGeneratorSystem());
            _systems.Add(new CameraSystem(width, height));
            _systems.Add(new RenderingSystem(width, height, shadowMap));
            _systems.Add(new CameraMovementSystem());
            _systems.Init();
            
            // Camera
            var camera = _world.NewEntity();
            camera.Replace(new Camera());
            camera.Replace(new Transform
            {
                Position = new Vector3(0, 0, -15),
            });
            
            // First Mesh
            var mesh = _world.NewEntity();
            mesh.Replace(new Transform
            {
                Position = new Vector3(0, -2, 0),
                Scale = new Vector3(0.5f, 0.5f, 0.5f)
            });
            mesh.Replace(new Material
            {
                ShaderToUse = "MainShader"
            });
            mesh.Replace(new Mesh
            {
                RootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources", "satis"),
                File = "constructor-sketchfab.obj"
            });
            
            // Second Mesh
            /*
            var twoMesh = _world.NewEntity();
            twoMesh.Replace(new Transform
            {
                Position = new Vector3(10, -2, 0),
                Scale = new Vector3(0.5f, 0.5f, 0.5f)
            });
            twoMesh.Replace(new Material
            {
                ShaderToUse = "MainShader"
            });
            twoMesh.Replace(new Mesh
            {
                RootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources", "satis"),
                File = "constructor-sketchfab.obj"
            });
            */
            //var twoMesh = mesh.Copy();
            //twoMesh.Get<Transform>().Position = new Vector3(10, -2, 0);

            base.OnLoad();
        }
        
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            
            //GL.DeleteProgram(_mainShader.handle);
            
            //_mainShader.Dispose();
            
            base.OnUnload();
        }
        
        protected override void OnResize(ResizeEventArgs resizeEventArgs)
        {
            GL.Viewport(0, 0, resizeEventArgs.Width, resizeEventArgs.Height);
            width = resizeEventArgs.Width;
            height = resizeEventArgs.Height;
            base.OnResize(resizeEventArgs);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs frameEventArgs)
        {
            InputManager.KeyboardState = KeyboardState.GetSnapshot();
            InputManager.MouseState = MouseState.GetSnapshot();
            var input = KeyboardState.GetSnapshot();

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            base.OnUpdateFrame(frameEventArgs);
        }
        
        protected override void OnRenderFrame(FrameEventArgs frameEventArgs)
        {
            _systems.Run();
            
            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
        }
    }
}