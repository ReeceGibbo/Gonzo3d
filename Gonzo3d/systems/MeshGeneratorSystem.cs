using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Assimp;
using Leopotam.Ecs;
using Mesh = Gonzo3d.components.Mesh;

namespace Gonzo3d.systems
{
    public class MeshGeneratorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter<Mesh> _meshFilter;

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

                if (!mesh.Loaded)
                {
                    LoadModel(ref mesh);
                }
                else if (mesh.Loaded && !mesh.Init)
                {
                    SetupGlModel(ref mesh);
                }
            }
        }

        private void LoadModel(ref Mesh mesh)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources", mesh.Path);

            var scene = _assimp.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            var sceneMesh = scene.Meshes[0];

            var vertices = sceneMesh.Vertices.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToList();
            var normals = sceneMesh.Normals.Select(vector => new Vector3(vector.X, vector.Y, vector.Z)).ToList();
            var uvs = sceneMesh.TextureCoordinateChannels[0].Select(vector => new Vector2(vector.X, vector.Y)).ToList();

            mesh.Vertices = vertices.ToArray();
            mesh.Normals = normals.ToArray();
            mesh.Uvs = uvs.ToArray();
            mesh.Indices = sceneMesh.GetUnsignedIndices();

            mesh.Loaded = true;
        }

        private void SetupGlModel(ref Mesh mesh)
        {
            
        }
    }
}