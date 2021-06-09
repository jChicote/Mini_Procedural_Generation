using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaderTerrainGeneration
{
    public struct MeshPointData
    {
        public Vector3 vert;
        public Vector3 normal;
        public Vector2 uv;
    }

    public struct TriangleSet
    {
        public Vector3 triangleA;
        public Vector3 triangleB;
    }

    public class TerrainGenerator : MonoBehaviour
    {
        public ComputeShader computeTerrainGen;
        public NoiseGenerator noiseGenerator;

        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected Mesh mesh;

        [Header("Terrain Cahracteristics")]
        public int resolution;
        public float maxHeight = 10;
        public int meshLineSize;
        private int lodIncrementStep;
        public int groundlevel;
        public int width = 2; // aspect will be 1:1

        public GameObject prefab;

        private List<GameObject> cubeObjects;

        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        private TriangleSet[] tris;
        public int[] meshTriangles;

        private int triangleIndex = 0;
        private int arrayBoundSize = 0;

        public void BuildTerrain()
        {
            mesh = new Mesh();
            int levelOfDetail = 0;
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            meshLineSize = (width - 1) / lodIncrementStep + 1;

            print(meshLineSize);

            noiseGenerator.GenerateNoiseSeed();
            float[] noiseData = noiseGenerator.CalculateNoise(width, new Vector3(0,0,0));

            int vertSize = sizeof(float) * 3; // size of the data (4 bytes for each float)
            ComputeBuffer vertBuffer = new ComputeBuffer(vertices.Length, vertSize);
            vertBuffer.SetData(vertices);

            int normalSize = sizeof(float) * 3; // size of the data (4 bytes for each float)
            ComputeBuffer normalBuffer = new ComputeBuffer(vertices.Length, normalSize);
            normalBuffer.SetData(normals);

            int uvSize = sizeof(float) * 2; // size of the data (4 bytes for each float)
            ComputeBuffer uvBuffer = new ComputeBuffer(vertices.Length, uvSize);
            uvBuffer.SetData(uv);

            int noiseSize = sizeof(float); // size of the data (4 bytes for each float)
            ComputeBuffer noiseBuffer = new ComputeBuffer(noiseData.Length, noiseSize);
            noiseBuffer.SetData(noiseData);

            int triangleSize = sizeof(float) * 6; // size of the data (4 bytes for each float)
            ComputeBuffer trisBuffer = new ComputeBuffer(tris.Length, triangleSize);
            trisBuffer.SetData(tris);

            computeTerrainGen.SetBuffer(0, "vertices", vertBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", noiseBuffer);
            computeTerrainGen.SetBuffer(0, "triangles", trisBuffer);
            computeTerrainGen.SetBuffer(0, "normal", normalBuffer);
            computeTerrainGen.SetBuffer(0, "uv", uvBuffer);
            computeTerrainGen.SetFloat("resolution", vertices.Length);
            computeTerrainGen.SetFloat("maxHeight", maxHeight);
            computeTerrainGen.SetInt("meshLineSize", meshLineSize);
            computeTerrainGen.SetFloat("meshSize", width);

            computeTerrainGen.Dispatch(0, vertices.Length / 10, 1, 1);

            vertBuffer.GetData(vertices);
            normalBuffer.GetData(normals);
            uvBuffer.GetData(uv);
            trisBuffer.GetData(tris);
            triangleIndex = 0;

            for (int i = 0; i < cubeObjects.Count; i++)
            {
                GameObject obj = cubeObjects[i];
                //MeshPointData pointData = data[i];
                Vector3 vert = vertices[i];
                TriangleSet triangleSet = tris[i];
                obj.transform.position = vert;
                IncludeNewTriangles(triangleSet);
            }

            // Manual Garbage collection of Buffers from memory
            vertBuffer.Dispose();
            normalBuffer.Dispose();
            uvBuffer.Dispose();
            noiseBuffer.Dispose();
            trisBuffer.Dispose();
        }

        int CalculateRow(int index)
        {
            float row = Mathf.Floor(index / width);
            return (int)row;
        }

        int CalculateColumn(int index, int row)
        {
            return index - row * width;
        }

        private void IncludeNewTriangles(TriangleSet set)
        {
            int row = CalculateRow(triangleIndex);
            int col = CalculateColumn(triangleIndex, row);

            if (triangleIndex >= arrayBoundSize) return;

                // first triangle
                AddTriangle(set.triangleA);

                // second triangle
                AddTriangle(set.triangleB);
        }

        private void AddTriangle(Vector3 triangle)
        {
            meshTriangles[triangleIndex] = (int) triangle.x;
            meshTriangles[triangleIndex + 1] = (int)triangle.y;
            meshTriangles[triangleIndex + 2] = (int)triangle.z;
            triangleIndex += 3;
        }

        private void PopulateMesh()
        {
            cubeObjects = new List<GameObject>();
            vertices = new Vector3[width * width];
            normals = new Vector3[width * width];
            uv = new Vector2[width * width];
            tris = new TriangleSet[width * width];
            meshTriangles = new int[(width - 1) * (width - 1) * 6];
            arrayBoundSize = (width - 1) * (width - 1) * 6;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Vector3 newPos = new Vector3(i, 0, j);
                    GameObject cube = Instantiate(prefab, transform.position + newPos, Quaternion.identity);
                    cubeObjects.Add(cube);

                    MeshPointData pointData = new MeshPointData
                    {
                        vert = newPos,
                        normal = Vector3.one,
                        uv = Vector2.zero
                    };

                    vertices[(i * width) + j] = newPos;
                    normals[(i * width) + j] = Vector3.one;
                    uv[(i * width) + j] = Vector2.zero;
                }
            }
        }

        public void RenderTerrain()
        {

        }

        /// <summary>
        /// Assigns mesh data items to the mesh object.
        /// </summary>
        /// <param name="mesh"></param>
        public void AssignMeshData()
        {
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

            print("Assigned Mesh");
        }

        public void VisualiseMesh()
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        private void OnGUI()
        {
            if (cubeObjects == null)
            {
                if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
                {
                    PopulateMesh();
                    BuildTerrain();
                    AssignMeshData();
                    VisualiseMesh();
                }
            }

            if (cubeObjects != null)
            {
                if (GUI.Button(new Rect(120, 0, 100, 50), "Regenerate"))
                {
                    BuildTerrain();
                    AssignMeshData();
                    VisualiseMesh();
                }
            }
        }
    }
}
