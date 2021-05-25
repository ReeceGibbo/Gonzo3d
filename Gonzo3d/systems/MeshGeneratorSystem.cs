using System.Collections.Generic;
using glTFLoader;
using glTFLoader.Schema;
using Leopotam.Ecs;
using Mesh = Gonzo3d.components.Mesh;

namespace Gonzo3d.systems
{
    public class MeshGeneratorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter<Mesh> _meshFilter;

        public void Init()
        {
            // https://github.com/KhronosGroup/glTF-CSharp-Loader
            // https://github.com/KhronosGroup/UnityGLTF
        }
        
        public void Run()
        {
            foreach (var i in _meshFilter)
            {
                ref var mesh = ref _meshFilter.Get1(i);

                if (!mesh.Init)
                {
                    var model = Interface.LoadModel(mesh.objPath);
                    var mainMesh = model.Meshes[0];

                    foreach (var primitive in mainMesh.Primitives)
                    {
                        
                    }
                }
            }
        }
    }
}