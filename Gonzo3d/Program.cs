using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gonzo3d
{
    class Program : GameWindow
    {
        float[] vertices = {
             0.5f,  0.5f, 0.0f,  // top right
             0.5f, -0.5f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f,  // bottom left
            -0.5f,  0.5f, 0.0f   // top left
        };
        
        uint[] indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };

        private Shader _shader;

        private int vertexBuffer;
        private int indicesBuffer;
        private int vao;
        
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }
        
        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            
            // Vertex Buffer
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            // VAO - Object & Linking Vertex Attributes
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            
            // Indices Buffer
            indicesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            
            // Reset buffer after use
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            // Load Shader
            _shader = new Shader("shaders/basic.vert", "shaders/basic.frag");
            _shader.Use();

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
        
        protected override void OnRenderFrame(FrameEventArgs frameEventArgs)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
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