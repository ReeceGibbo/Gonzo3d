using System.Collections.Generic;
using Gonzo3d.components;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Gonzo3d.systems
{
    public class RenderingSystem : IEcsInitSystem, IEcsRunSystem
    {
        
        private EcsWorld _world;
        
        private EcsFilter<Camera, Transform> _cameraFilter;
        private EcsFilter<Mesh, Material, Transform> _meshFilter;

        private int _width;
        private int _height;

        private ShadowMap _shadowMap;

        public RenderingSystem(int width, int height, ShadowMap shadowMap)
        {
            _width = width;
            _height = height;
            _shadowMap = shadowMap;
        }
        
        public void Init()
        {
            
        }
        
        public void Run()
        {
            // First get our main camera
            if (_cameraFilter.IsEmpty())
                return;
            
            ref var camera = ref _cameraFilter.Get1(0);
            ref var cameraTransform = ref _cameraFilter.Get2(0);

            foreach (var i in _meshFilter)
            {
                ref var mesh = ref _meshFilter.Get1(i);
                ref var material = ref _meshFilter.Get2(i);
                ref var transform = ref _meshFilter.Get3(i);
                
                ShadowPass(ref mesh, ref material, ref transform, ref camera, ref cameraTransform);
                NormalPass(ref mesh, ref material, ref transform, ref camera, ref cameraTransform);
            }
        }

        private void DrawMesh(ref Mesh mesh, ref Material material, ref Transform transform, Shader shader)
        {
            GL.BindVertexArray(mesh.Vao);

            var model = Matrix4.CreateTranslation(transform.Position.X, transform.Position.Y, transform.Position.Z);
            
            GL.UseProgram(shader.Handle);
            shader.SetMatrix4("model", model);
            
            var texture = TextureManager.GetTexture(material.DiffuseTexture);
            texture.Use(TextureUnit.Texture0);

            GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void NormalPass(ref Mesh mesh, ref Material material, ref Transform transform, ref Camera camera, ref Transform cameraTransform)
        {
            GL.Viewport(0, 0, _width, _height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var shader = ShaderManager.GetShader(material.ShaderToUse);
            GL.UseProgram(shader.Handle);
            SetShadowMatrix(ref cameraTransform, shader);

            shader.SetMatrix4("view", camera.View);
            shader.SetMatrix4("projection", camera.Projection);
            shader.SetVector3("lightDirection", new Vector3(0.5f, -0.8f, -0.8f));
            
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _shadowMap.DepthMap);
            
            DrawMesh(ref mesh, ref material, ref transform, shader);
        }

        private void ShadowPass(ref Mesh mesh, ref Material material, ref Transform transform, ref Camera camera, ref Transform cameraTransform)
        {
            GL.Viewport(0, 0, ShadowMap.ShadowMapWidth, ShadowMap.ShadowMapHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _shadowMap.ShadowMapFbo);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            var shader = ShaderManager.GetShader("ShadowShader");
            GL.UseProgram(shader.Handle);
            SetShadowMatrix(ref cameraTransform, shader);
            
            DrawMesh(ref mesh, ref material, ref transform, shader);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        public void SetShadowMatrix(ref Transform cameraTransform, Shader shader)
        {
            var lightProjection = Matrix4.CreateOrthographic(20.0f, 20.0f, 0.1f, 100.0f);
            //var lightView = Matrix4.LookAt(-cameraTransform.Position, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            var lightDir = new Vector3(0.5f, -0.8f, -0.8f);
            var lightView = Matrix4.LookAt(new Vector3(0, 0, 0), lightDir, new Vector3(0, 1, 0));
            var lightSpaceMatrix = lightView * lightProjection;
            
            shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
        }
        
    }
}