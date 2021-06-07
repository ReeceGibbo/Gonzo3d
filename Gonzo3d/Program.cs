using System;
using System.IO;
using System.Reflection;
using Gonzo3d.components;
using Gonzo3d.systems;
using Leopotam.Ecs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gonzo3d
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(800, 800),
                Title = "Reece Testing",
            };

            using (var game = new Game(new GameWindowSettings(), nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}