using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProceduralGeneration.MeshGeneration;

namespace ProceduralGeneration.TerrainGeneration
{
    public class OfflineTerrainGenerator : BaseLandGenerator
    {
        [Range(0, 6)]
        [SerializeField] protected int levelOfDetail;
        private MeshData meshData;

        private int meshVertSize;
        private int lodIncrementStep;

        private void Start()
        {
            meshData = new MeshData();
            meshCollider = this.GetComponent<MeshCollider>();
            vertices = new Vector3[4];
            normals = new Vector3[4];
            uv = new Vector2[4];
        }

        public override void GenerateMap()
        {
            Mesh mesh = new Mesh();
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            meshVertSize = (mapSize - 1) / lodIncrementStep + 1;
            //float[] noiseMap = noiseGenerator.CalculateNoise(mapSize);

            //GenerateMeshData(noiseMap);
            AssignMeshData(mesh);
            regenerate = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noiseData"></param>
        public override void GenerateMeshData(float[] noiseData)
        {

            meshData.SetMeshArrays(meshVertSize);

            for (int index = 0, row = 0; row < mapSize; row += lodIncrementStep)
            {
                for (int col = 0; col < mapSize; index++, col += lodIncrementStep)
                {
                    meshData.vertices[index] = new Vector3(col, CalculateHeight(noiseData[row * mapSize + col]), row);
                    meshData.normals[index] = -Vector3.forward;
                    meshData.uv[index] = new Vector2(col, row);

                    if (col < mapSize - 1 && row < mapSize - 1)
                    {
                        meshData.AddTriangle(index, index + meshVertSize, index + 1);
                        meshData.AddTriangle(index + 1, index + meshVertSize, index + meshVertSize + 1);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public override void AssignMeshData(Mesh mesh)
        {
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.normals = meshData.normals;
            mesh.uv = meshData.uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
