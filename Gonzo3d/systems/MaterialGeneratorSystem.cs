using System.Collections.Generic;
using Gonzo3d.components;
using Leopotam.Ecs;

namespace Gonzo3d.systems
{
    public class MaterialGeneratorSystem : IEcsInitSystem, IEcsRunSystem
    {
        
        private Dictionary<string, int> _textures;
        private Dictionary<string, int> _shaders;
        
        private EcsWorld _world;
        
        public void Init()
        {
            _textures = new Dictionary<string, int>();
            _shaders = new Dictionary<string, int>();
        }

        public void Run()
        {
            
        }
    }
}