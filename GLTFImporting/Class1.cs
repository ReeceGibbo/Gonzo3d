using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assimp;
using glTFLoader;

namespace GLTFImporting
{
    public class Class1
    {

        public Class1()
        {
            /*
            var model = Interface.LoadModel("Triangle.gltf");

            // For each buffer, decode the data as a binary array and store.
            var buffers = new List<byte[]>();

            foreach (var b in model.Buffers)
            {
                if (b.Uri.StartsWith("data:application/octet-stream;base64,"))
                {
                    var b64 = b.Uri[37..];
                    buffers.Add(Convert.FromBase64String(b64));
                }
            }

            // For each mesh
            foreach (var mesh in model.Meshes)
            {
                var positionId = 0;
                var indicesId = 0;
                
                // Build data IDs
                foreach (var primitive in mesh.Primitives)
                {
                    if (primitive.Attributes.ContainsKey("POSITION"))
                    {
                        positionId = primitive.Attributes["POSITION"];
                    }
                    if (primitive.Indices.HasValue)
                    {
                        indicesId = primitive.Indices.Value;
                    }
                }
                
                // Get accessors for IDs
                var positionAccessor = model.Accessors[positionId];
                var indicesAccessor = model.Accessors[indicesId];

                var positionBufferView = model.BufferViews[positionAccessor.BufferView.Value];
                var indicesBufferView = model.BufferViews[indicesAccessor.BufferView.Value];
                
                // Get positions
                var positions = new float[positionAccessor.Count * 3];
                Buffer.BlockCopy(buffers[positionBufferView.Buffer].ToArray(),
                    positionAccessor.ByteOffset, positions, 0, positionBufferView.ByteLength);

                foreach (var pos in positions)
                {
                    Debug.WriteLine(pos);
                }

            }
            // Get accessors for both
            // Get buffer view for position and process data from buffer
            // Get buffer view for indicies and process data from buffer
            */

            var importer = new AssimpContext();
            var scene = importer.ImportFile("Triangle.gltf");

            var mesh = scene.Meshes[0];
        }
        
        static void Main(string[] args)
        {
            var test = new Class1();
        }
    }
}