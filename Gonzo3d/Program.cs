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
using TextureWrapMode = Assimp.TextureWrapMode;

namespace Gonzo3d
{
    class Program : GameWindow
    {
        // Model
        private readonly float[] _vertices;
        private float[] _normals;
        private uint[] _indices;

        private Shader _mainShader;
        private Shader _shadowShader;
        
        private Texture _texture;
        
        private int vertexBuffer;
        private int indicesBuffer;
        private int vao;
        
        // Shaders
        private int shadowMapFbo;
        private int depthMap;
        private const int SHADOW_MAP_WIDTH = 4096;
        private const int SHADOW_MAP_HEIGHT = 4096;

        private Vector3 lightPos = new Vector3(0, 0, 15);
        private Vector3 meshPos = new Vector3(0, -2, 0);
        private Vector3 viewPos = new Vector3(0, 0, -15);

        private int width;
        private int height;

        private Matrix4 projection;
        private Matrix4 model;
        private Matrix4 view;
        
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            width = nativeWindowSettings.Size.X;
            height = nativeWindowSettings.Size.Y;

            var importer = new AssimpContext();

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources",
                "satis/constructor-sketchfab.obj");
            var scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            var mesh = scene.Meshes[0];

            _vertices = new float[mesh.VertexCount * 8];

            var i = 0;
            for (var index = 0; index < mesh.Vertices.Count; index++)
            {
                var vector = mesh.Vertices[index];
                var normal = mesh.Normals[index];
                var textureCoords = mesh.TextureCoordinateChannels[0][index];
                
                _vertices[i] = vector.X;
                _vertices[i+1] = vector.Y;
                _vertices[i+2] = vector.Z;
                _vertices[i+3] = textureCoords.X;
                _vertices[i+4] = textureCoords.Y;
                _vertices[i+5] = normal.X;
                _vertices[i+6] = normal.Y;
                _vertices[i+7] = normal.Z;
                i += 8;
            }

            _indices = mesh.GetUnsignedIndices();
            
            // Texture
            var texPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources",
                "satis/constructor-sketchfab.png");
            _texture = new Texture(texPath);
            _texture.Use(TextureUnit.Texture0);
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
            _mainShader = new Shader("shaders/basic.vert", "shaders/basic.frag");
            _mainShader.Use();

            // Load Texture
            _mainShader.SetInt("diffuseTexture", 0);
            
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
            var vertexLocation = GL.GetAttribLocation(_mainShader.handle, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var texCoordLocation = GL.GetAttribLocation(_mainShader.handle, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var normalLocation = GL.GetAttribLocation(_mainShader.handle, "aNormals");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));

            // Shadows
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

            // Reset buffer after use
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

            GL.DeleteProgram(_mainShader.handle);
            
            _mainShader.Dispose();
            
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
            var input = KeyboardState.GetSnapshot();

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            base.OnUpdateFrame(frameEventArgs);
        }
        
        protected override void OnRenderFrame(FrameEventArgs frameEventArgs)
        {
            DrawShadowPass();
            DrawNormalPass();
            
            SwapBuffers();
            base.OnRenderFrame(frameEventArgs);
        }

        public void SetShadowMatrix(Shader shader)
        {
            var lightProjection = Matrix4.CreateOrthographic(20.0f, 20.0f, 1.0f, 100.0f);
            var lightView = Matrix4.LookAt(lightPos, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            var lightSpaceMatrix = lightView * lightProjection;
            
            shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
        }

        public void DrawShadowPass()
        {
            GL.Viewport(0, 0, SHADOW_MAP_WIDTH, SHADOW_MAP_HEIGHT);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFbo);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture.Handle);
            
            _shadowShader.Use();
            SetShadowMatrix(_shadowShader);
            
            DrawMesh(_shadowShader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        public void DrawNormalPass()
        {
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _mainShader.Use();
            SetShadowMatrix(_mainShader);
            
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);
            view = Matrix4.CreateTranslation(viewPos);
            _mainShader.SetMatrix4("view", view);
            _mainShader.SetMatrix4("projection", projection);


            //_mainShader.SetVector3("viewPos", viewPos);
            _mainShader.SetVector3("lightPos", lightPos);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture.Handle);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, depthMap);

            DrawMesh(_mainShader);
        }

        private float value = 0;
        public void DrawMesh(Shader shader)
        {
            value += 0.00001f;
            GL.BindVertexArray(vao);

            model = Matrix4.CreateTranslation(meshPos) * Matrix4.CreateRotationY(value) * Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            shader.SetMatrix4("model", model);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
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