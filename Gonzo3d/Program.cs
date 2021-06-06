using System;
using System.IO;
using System.Reflection;
using Gonzo3d.components;
using Gonzo3d.systems;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gonzo3d
{
    class Program : GameWindow
    {
        // Shaders
        private int shadowMapFbo;
        private int depthMap;
        private const int SHADOW_MAP_WIDTH = 4096;
        private const int SHADOW_MAP_HEIGHT = 4096;

        private EcsWorld _world;
        private EcsSystems _systems;

        //private Vector3 lightDirection = new Vector3(0.5f, -0.8f, -0.8f);
        //private Vector3 meshPos = new Vector3(0, -2, 0);
        //private Vector3 viewPos = new Vector3(0, 0, -15);

        private int width;
        private int height;
        
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            width = nativeWindowSettings.Size.X;
            height = nativeWindowSettings.Size.Y;
            
            // Texture
            /*
            var texPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources",
                "satis/", scene.Materials[1].TextureDiffuse.FilePath);
            _texture = new Texture(texPath);
            _texture.Use(TextureUnit.Texture0);
            */
        }
        
        protected override void OnLoad()
        {
            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems.Add(new ShaderGeneratorSystem());
            _systems.Add(new MeshGeneratorSystem());
            _systems.Add(new MaterialGeneratorSystem());
            _systems.Add(new CameraSystem(width, height));
            _systems.Add(new RenderingSystem(width, height));
            _systems.Add(new CameraMovementSystem());
            _systems.Init();

            // Shaders
            var mainShader = _world.NewEntity();
            mainShader.Replace(new Shader
            {
                Name = "MainShader",
                FragmentPath = "shaders/simple.frag",
                VertexPath = "shaders/simple.vert"
            });
            
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
                Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources",
                    "satis/constructor-sketchfab.obj")
            });

            // Shadows
            /*
            shadowMapFbo = GL.GenFramebuffer();
            depthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
                SHADOW_MAP_WIDTH, SHADOW_MAP_HEIGHT, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, depthMap, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            _mainShader.SetInt("shadowMap", 1);

            _shadowShader = new Shader("shaders/shadow.vert", "shaders/shadow.frag");
            */
            
            // Reset buffer after use
            //GL.BindVertexArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            //GL.UseProgram(0);

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
            //DrawShadowPass();
            //DrawNormalPass();
            
            _systems.Run();
            
            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
        }
        
        public void SetShadowMatrix()
        {
            /*
            var lightProjection = Matrix4.CreateOrthographic(20.0f, 20.0f, 0.1f, 100.0f);
            var lightView = Matrix4.LookAt(-viewPos, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            var lightSpaceMatrix = lightView * lightProjection;
            
            shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
            */
        }

        public void DrawShadowPass()
        {
            /*
            GL.Viewport(0, 0, SHADOW_MAP_WIDTH, SHADOW_MAP_HEIGHT);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFbo);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture.Handle);
            
            _shadowShader.Use();
            SetShadowMatrix(_shadowShader);
            
            DrawMesh(_shadowShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            */
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(800, 800),
                Title = "Reece Testing",
            };
            
            using (var game = new Program(new GameWindowSettings(), nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}