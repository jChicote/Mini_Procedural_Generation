using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralGeneration.TerrainGeneration
{
    public class OfflineTerrainGenerator : BaseLandGenerator
    {
        [Range(0, 6)]
        [SerializeField] protected int levelOfDetail;

        private int meshVertSize;
        private int lodIncrementStep;
        int triangleIndex = 0;

        private void Start()
        {
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
            Debug.Log("Increment size: " + lodIncrementStep);
            Debug.Log("Mesh size: " + meshVertSize);
            edgeLength = 1;
            float[] noiseMap = noiseGenerator.CalculateNoise(mapSize);

            GenerateMeshData(noiseMap);
            //DetermineMeshTriangles();
            AssignMeshData(mesh);
            regenerate = false;
        }

        public override void GenerateMeshData(float[] noiseData)
        {
            triangles = new int[(meshVertSize - 1) * (meshVertSize - 1) * 6];
            triangleIndex = 0;
            Debug.Log("Array Length is: " + triangles.Length);
            vertices = new Vector3[(meshVertSize) * (meshVertSize )];
            normals = new Vector3[(meshVertSize) * (meshVertSize )];
            uv = new Vector2[(meshVertSize) * (meshVertSize)];
            for (int index = 0, row = 0; row < mapSize; row += lodIncrementStep)
            {
                for (int col = 0; col < mapSize; index++, col += lodIncrementStep)
                {
                    vertices[index] = new Vector3(col, CalculateHeight(noiseData[row * mapSize + col]), row);
                    normals[index] = -Vector3.forward;
                    uv[index] = new Vector2(col, row);

                    if (col < mapSize - 1 && row < mapSize - 1)
                    {
                        //AddTriangle(index, index + meshVertSize + 1, index + meshVertSize);
                        //AddTriangle(index + meshVertSize + 1, index, index + 1);

                        AddTriangle(index, index + meshVertSize, index + 1);
                        AddTriangle(index + 1, index + meshVertSize, index + meshVertSize + 1);
                    }
                    //Debug.Log(triangleIndex);
                }
            }

            //Debug.Log(vertices.Length);
        }

        public override void DetermineMeshTriangles()
        {
            triangles = new int[meshVertSize-1 * meshVertSize-1 * 6];
            
            int triangleIndex = 0;

            for (int row = 0, vi = 0, ti = 0; row < meshVertSize; row++, vi++)
            {
                for (int col = 0; col < meshVertSize; col++, ti += 6, vi++)
                {
                    //Debug.Log(ti);
                    //Debug.Log("Length is: " + triangles.Length);

                    // Lower left
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;

                    // Upper Right
                    triangles[ti + 4] = triangles[ti + 1] = vi + meshVertSize + 1;
                    triangles[ti + 5] = vi + meshVertSize + 2;
                }
            }

            /*for (int index = 0, row = 0; row < meshVertSize; row++)
            {
                for (int col = 0; col < meshVertSize; index += 6, col++)
                {
                    //lower left triangle
                    triangles[index] = row * meshVertSize + col;
                    triangles[index + 1] = (row + 1) * meshVertSize + col;
                    triangles[index + 2] = row * meshVertSize + col + 1;

                    //upper right tringle
                    triangles[index + 3] = row * meshVertSize + col + 1;
                    triangles[index + 4] = (row + 1) * meshVertSize + col;
                    triangles[index + 5] = (row + 1) * meshVertSize + col + 2;
                }
            }*/
        }

        private void AddTriangle(int a, int b, int c)
        {
            //Debug.Log(triangleIndex);
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }
}
