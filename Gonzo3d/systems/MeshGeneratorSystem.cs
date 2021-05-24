using System.Collections.Generic;
using Gonzo3d.components;
using Leopotam.Ecs;

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
            }
        }
    }
}