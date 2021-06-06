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
        
        private EcsFilter<Camera> _cameraFilter;
        private EcsFilter<Mesh, Material, Transform> _meshFilter;
        private EcsFilter<Shader> _shaderFilter;

        private int _width;
        private int _height;

        public RenderingSystem(int width, int height)
        {
            _width = width;
            _height = height;
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

            foreach (var i in _meshFilter)
            {
                ref var mesh = ref _meshFilter.Get1(i);
                ref var material = ref _meshFilter.Get2(i);
                ref var transform = ref _meshFilter.Get3(i);

                foreach (var s in _shaderFilter)
                {
                    ref var shader = ref _shaderFilter.Get1(s);
                    if (material.ShaderToUse == shader.Name && shader.Compiled)
                    {
                        NormalPass(ref mesh, ref material, ref transform, ref camera, ref shader);
                    }
                }
                
            }
        }

        private void DrawMesh(ref Mesh mesh, ref Material material, ref Transform transform, ref Shader shader)
        {
            GL.BindVertexArray(mesh.Vao);

            var model = Matrix4.CreateTranslation(transform.Position.X, transform.Position.Y, transform.Position.Z);
            
            GL.UseProgram(shader.Handle);
            ShaderHelper.SetMatrix4(ref shader, "model", model);

            GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void NormalPass(ref Mesh mesh, ref Material material, ref Transform transform, ref Camera camera, ref Shader shader)
        {
            GL.Viewport(0, 0, _width, _height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shader.Handle);
            //SetShadowMatrix(_mainShader);

            ShaderHelper.SetMatrix4(ref shader, "view", camera.View);
            ShaderHelper.SetMatrix4(ref shader, "projection", camera.Projection);

            ShaderHelper.SetVector3(ref shader, "lightDirection", new Vector3(0.5f, -0.8f, -0.8f));
            
            DrawMesh(ref mesh, ref material, ref transform, ref shader);
        }

        private void ShadowPass(ref Mesh mesh, ref Material material, ref Camera camera)
        {

        }
        
        public void SetShadowMatrix(ref Material material, ref Camera camera)
        {
            
            var lightProjection = Matrix4.CreateOrthographic(20.0f, 20.0f, 0.1f, 100.0f);
            //var lightView = Matrix4.LookAt(-viewPos, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //var lightSpaceMatrix = lightView * lightProjection;
            
            //ShaderHelper.SetMatrix4(shader.Handle, "lightSpaceMatrix", lightProjection);
        }
        
    }
}