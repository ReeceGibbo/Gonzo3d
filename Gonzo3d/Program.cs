using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Gonzo3d
{
    class Program : GameWindow
    {
        // Model
        private readonly float[] _vertices;
        private float[] _normals;
        private uint[] _indices;

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

            var importer = new AssimpContext();

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources",
                "teapot/teapot.obj");
            var scene = importer.ImportFile(filePath);
            
            var mesh = scene.Meshes[0];

            _vertices = new float[mesh.VertexCount * 7];

            var i = 0;
            for (var index = 0; index < mesh.Vertices.Count; index++)
            {
                var vector = mesh.Vertices[index];
                var normal = mesh.Normals[index];
                
                _vertices[i] = vector.X;
                _vertices[i+1] = vector.Y;
                _vertices[i+2] = vector.Z;
                _vertices[i+3] = 0;
                _vertices[i+4] = 0;
                _vertices[i+5] = normal.X;
                _vertices[i+6] = normal.Y;
                i += 7;
            }

            _indices = mesh.GetUnsignedIndices();
        }
        
        protected override void OnLoad()
        {
            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

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
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            
            // Linking Vertex Attributes
            var vertexLocation = GL.GetAttribLocation(_shader.handle, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            var texCoordLocation = GL.GetAttribLocation(_shader.handle, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

            var normalLocation = GL.GetAttribLocation(_shader.handle, "aNormals");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 5 * sizeof(float));

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            value -= 0.0001f;
            // New drawing method with Vertex Arrays
            
            // Bind Vertex Array
            GL.BindVertexArray(vao);
            
            // Bind Texture
            _texture.Use(TextureUnit.Texture0);

            // Bind Shader
            _shader.Use();

            var modelView = Matrix4.CreateRotationY(value);
            
            _shader.SetMatrix4("model", modelView);
            view = Matrix4.CreateTranslation(0, 0, -15f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetVector3("lightPos", new Vector3(0, -15f, -15f));
            
            // Bind Index Buffer
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBuffer);
            
            // DRAW CALL
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
            
            // FPS Counter
            _framesPerSecondCounter.Draw();

            
            test++;
            if (test > 1200)
                test = 0;
            
            if (test == 1200)
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