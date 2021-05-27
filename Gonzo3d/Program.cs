using System;
using System.Diagnostics;
using System.Timers;
using glTFLoader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gonzo3d
{
    class Program : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
            0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private uint[] indices = {
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };

        private Shader _shader;
        private Texture _texture;
        
        private int vertexBuffer;
        private int indicesBuffer;
        private int vao;

        private float width;
        private float height;

        private Matrix4 projection;
        private Matrix4 model;
        private Matrix4 view;

        private FramesPerSecondCounter _framesPerSecondCounter;
        
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            width = nativeWindowSettings.Size.X;
            height = nativeWindowSettings.Size.Y;

            _framesPerSecondCounter = new FramesPerSecondCounter();
            
            //var model = Interface.LoadModel("resources/Triangle.gltf");

            //if (model.Buffers[0].Uri.StartsWith("data:application/octet-stream;base64,"))
            
            //byte[] bufferData;
            // Get scene 0
            // Get Node 0
            // Get Mesh 0
            // Get Mesh POSITION Buffer Int
            // Get Mesh INDICES Buffer Int
            // Get Accessor objects for POSITION & INDICES
            // Use Accessor Information to get BufferViews & Buffer info.
        }
        
        protected override void OnLoad()
        {
            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);

            // Camera Matrix
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);
            model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            
            // Load Shader
            _shader = new Shader("shaders/basic.vert", "shaders/basic.frag");
            _shader.Use();

            // Load Texture
            _texture = new Texture("resources/container.png");
            _texture.Use(TextureUnit.Texture0);
            
            _shader.SetInt("mainTex", 0);
            
            // Vertex Array Object
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            
            // Vertex Buffer
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            
            // Indices Buffer
            indicesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            // Linking Vertex Attributes
            var vertexLocation = GL.GetAttribLocation(_shader.handle, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = GL.GetAttribLocation(_shader.handle, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // Reset buffer after use
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.UseProgram(0);

            base.OnLoad();
        }
        
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indicesBuffer);
            GL.DeleteVertexArray(vao);

            GL.DeleteProgram(_shader.handle);
            
            _shader.Dispose();
            
            base.OnUnload();
        }
        
        protected override void OnResize(ResizeEventArgs resizeEventArgs)
        {
            GL.Viewport(0, 0, resizeEventArgs.Width, resizeEventArgs.Height);
            base.OnResize(resizeEventArgs);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs frameEventArgs)
        {
            var input = KeyboardState.GetSnapshot();

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            base.OnUpdateFrame(frameEventArgs);
        }

        private float value = -1f;
        private int test = 0;
        
        protected override void OnRenderFrame(FrameEventArgs frameEventArgs)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            value += 0.00001f;

            test++;
            if (test > 1200)
                test = 0;
            // New drawing method with Vertex Arrays
            
            // Bind Vertex Array
            GL.BindVertexArray(vao);
            
            // Bind Texture
            _texture.Use(TextureUnit.Texture0);

            // Bind Shader
            _shader.Use();
            
            _shader.SetMatrix4("model", model);
            view = Matrix4.CreateTranslation(value, value, -3f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            
            // Bind Index Buffer
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBuffer);
            
            // DRAW CALL
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
            
            // FPS Counter
            _framesPerSecondCounter.Draw();

            if (test == 120)
                Debug.WriteLine($"FPS: {_framesPerSecondCounter.FramesPerSecond}");
            
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var game = new Program(new GameWindowSettings(), NativeWindowSettings.Default))
            {
                game.Run();
            }
        }
    }
}