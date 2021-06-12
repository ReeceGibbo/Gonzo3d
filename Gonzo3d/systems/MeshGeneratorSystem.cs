using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Assimp;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;
using Material = Gonzo3d.components.Material;
using Mesh = Gonzo3d.components.Mesh;

namespace Gonzo3d.systems
{
    public class MeshGeneratorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter<Mesh, Material> _meshFilter;

        private AssimpContext _assimp;
        
        public void Init()
        {
            _assimp = new AssimpContext();
        }
        
        public void Run()
        {
            foreach (var i in _meshFilter)
            {
                ref var mesh = ref _meshFilter.Get1(i);
                ref var material = ref _meshFilter.Get2(i);

                if (!mesh.Loaded)
                {
                    LoadModel(ref mesh, ref material);
                }
                else if (mesh.Loaded && !mesh.Init)
                {
                    SetupGlModel(ref mesh);
                }
            }
        }

        private void LoadModel(ref Mesh mesh, ref Material material)
        {
            var modelPath = Path.Combine(mesh.RootPath, mesh.File);

            var scene = _assimp.ImportFile(modelPath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.OptimizeMeshes);
            var sceneMesh = scene.Meshes[0];

            var vertices = sceneMesh.Vertices.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToArray();
            var normals = sceneMesh.Normals.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToArray();
            var uvs = sceneMesh.TextureCoordinateChannels[0].Select(vector => new Vector2(vector.X, vector.Y)).ToArray();

            mesh.Vertices = vertices;
            mesh.Normals = normals;
            mesh.Uvs = uvs;
            mesh.Indices = sceneMesh.GetUnsignedIndices();
            
            // Load Textures and Material from Model
            var matIndex = sceneMesh.MaterialIndex;
            var mat = scene.Materials[matIndex];

            if (mat.HasTextureDiffuse)
            {
                if (!TextureManager.HasTexture(mat.TextureDiffuse.FilePath))
                {
                    var texturePath = Path.Combine(mesh.RootPath, mat.TextureDiffuse.FilePath);
                    TextureManager.CreateTexture(mat.TextureDiffuse.FilePath, texturePath);
                    material.DiffuseTexture = mat.TextureDiffuse.FilePath;
                }
            }

            mesh.Loaded = true;
        }

        private void SetupGlModel(ref Mesh mesh)
        {
            // Vertex Array Object - Contains multiple buffers
            mesh.Vao = GL.GenVertexArray();
            GL.BindVertexArray(mesh.Vao);

            // Vertex Buffer - contains all MESH data
            mesh.Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.Vbo);

            // Build Mesh Data for GPU
            var meshData = new float[mesh.Vertices.Length * 8];

            for (int index = 0; index < mesh.Vertices.Length; index++)
            {
                var vertex = mesh.Vertices[index];
                var normal = mesh.Normals[index];
                var uv = mesh.Uvs[index];

                var pos = (index * 8);

                // Vertices
                meshData[pos]     = vertex.X;
                meshData[pos + 1] = vertex.Y;
                meshData[pos + 2] = vertex.Z;
                
                // UVs
                meshData[pos + 3] = uv.X;
                meshData[pos + 4] = uv.Y;
                
                // Normals
                meshData[pos + 5] = normal.X;
                meshData[pos + 6] = normal.Y;
                meshData[pos + 7] = normal.Z;
            }
            
            GL.BufferData(BufferTarget.ArrayBuffer, meshData.Length * sizeof(float), meshData, BufferUsageHint.StaticDraw);
            
            // Indices Buffer
            mesh.Ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Length * sizeof(uint), mesh.Indices, BufferUsageHint.StaticDraw);
            
            // Completed Initialization of model
            mesh.Init = true;
        }
    }
}