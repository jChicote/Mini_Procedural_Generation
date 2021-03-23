using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralGeneration.TerrainGeneration
{
    public class OfflineTerrainGenerator : BaseLandGenerator
    {
        private void Start()
        {
            meshCollider = this.GetComponent<MeshCollider>();
            vertices = new Vector3[4];
            normals = new Vector3[4];
            uv = new Vector2[4];

            GenerateBasicMap();
        }
    }
}
