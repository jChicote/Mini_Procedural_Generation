using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.MeshGeneration
{
    [System.Serializable]
    public class MeshData
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        public int[] triangles;

        private int triangleIndex;

        /// <summary>
        /// Sets the mesh arrays and initialises the mesh data
        /// </summary>
        public void SetMeshArrays(int meshSize)
        {
            triangleIndex = 0;

            vertices = new Vector3[meshSize * meshSize];
            normals = new Vector3[meshSize * meshSize];
            uv = new Vector2[meshSize * meshSize];
            triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
        }

        /// <summary>
        /// Adds triangle into the array, configuration can follow clockwise and anti-clockwisse direcitons
        /// </summary>
        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }
}
