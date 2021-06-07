using System.Collections.Generic;
using Gonzo3d.components;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;

namespace Gonzo3d.systems
{
    public class MaterialGeneratorSystem : IEcsRunSystem
    {
        
        private EcsWorld _world;
        private EcsFilter<Mesh, Material> _materialFilter;

        public void Run()
        {
            foreach (var i in _materialFilter)
            {
                ref var mesh = ref _materialFilter.Get1(i);
                ref var material = ref _materialFilter.Get2(i);

                if (mesh.Init && !material.Init)
                {
                    SetupMaterial(ref mesh, ref material);
                }
            }
        }

        private void SetupMaterial(ref Mesh mesh, ref Material material)
        {
            var shader = ShaderManager.GetShader(material.ShaderToUse);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.Vbo);

            // Tell the GPU where the vertices data is
            material.VertexLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.EnableVertexAttribArray(material.VertexLocation);
            GL.VertexAttribPointer(material.VertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                    
            // Texture Coords
            material.TexCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.EnableVertexAttribArray(material.TexCoordLocation);
            GL.VertexAttribPointer(material.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                    
            // Normals
            material.NormalLocation = GL.GetAttribLocation(shader.Handle, "aNormals");
            GL.EnableVertexAttribArray(material.NormalLocation);
            GL.VertexAttribPointer(material.NormalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
        }
    }
}