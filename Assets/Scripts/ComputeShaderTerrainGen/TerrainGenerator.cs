using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public struct MeshComputeBuffers
    {
        public ComputeBuffer vertBuffer;
        public ComputeBuffer normalBuffer;
        public ComputeBuffer uvBuffer;
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer trisBuffer;
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
        public float minHeight = 0;
        public int vertPerSide;
        private int lodIncrementStep;
        public int groundlevel;
        public int width = 2; // aspect will be 1:1
        [Range(0, 6)]
        public int levelOfDetail = 0;

        public Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private TriangleSet[] quads;
        public int[] meshTriangles;

        private int triangleIndex = 0;
        private int arrayBoundSize = 0;

        private void PopulateMeshAttributes()
        {
            // This can only work as long as values remain even
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            vertPerSide = (width - 1) / lodIncrementStep + 1;

            vertices = new Vector3[vertPerSide * vertPerSide];
            normals = new Vector3[vertPerSide * vertPerSide];
            uv = new Vector2[vertPerSide * vertPerSide];
            quads = new TriangleSet[vertPerSide * vertPerSide];
            meshTriangles = new int[(vertPerSide - 1) * (vertPerSide - 1) * 6];
            arrayBoundSize = (vertPerSide - 1) * (vertPerSide - 1) * 6;
        }

        public void BuildTerrain()
        {
            mesh = new Mesh();

            noiseGenerator.GenerateNoiseSeed();
            float[] noiseData = noiseGenerator.CalculateNoise(width, new Vector3(0,0,0));

            MeshComputeBuffers meshComputeBuffers = CreateNewMeshBuffers(noiseData);
            SetComputeShaderBuffers(meshComputeBuffers);
            computeTerrainGen.Dispatch(0, vertices.Length / 10, 1, 1);

            RetrieveDataFromComputeShader(meshComputeBuffers);
            IndexTriangleData();

            // Manual Garbage collection of Buffers from memory
            DisposeBuffersIntoGarbageCollection(meshComputeBuffers);
        }

        private MeshComputeBuffers CreateNewMeshBuffers(float[] noiseData)
        {
            MeshComputeBuffers meshBuffers = new MeshComputeBuffers();

            int vertSize = sizeof(float) * 3; // size of the data (4 bytes for each float)
            ComputeBuffer vertBuffer = new ComputeBuffer(vertices.Length, vertSize);
            vertBuffer.SetData(vertices);
            meshBuffers.vertBuffer = vertBuffer;

            int normalSize = sizeof(float) * 3; // size of the data (4 bytes for each float)
            ComputeBuffer normalBuffer = new ComputeBuffer(vertices.Length, normalSize);
            normalBuffer.SetData(normals);
            meshBuffers.normalBuffer = normalBuffer;

            int uvSize = sizeof(float) * 2; // size of the data (4 bytes for each float)
            ComputeBuffer uvBuffer = new ComputeBuffer(vertices.Length, uvSize);
            uvBuffer.SetData(uv);
            meshBuffers.uvBuffer = uvBuffer;

            int noiseSize = sizeof(float); // size of the data (4 bytes for each float)
            ComputeBuffer noiseBuffer = new ComputeBuffer(noiseData.Length, noiseSize);
            noiseBuffer.SetData(noiseData);
            meshBuffers.noiseBuffer = noiseBuffer;

            int triangleSize = sizeof(float) * 6; // size of the data (4 bytes for each float)
            ComputeBuffer trisBuffer = new ComputeBuffer(quads.Length, triangleSize);
            trisBuffer.SetData(quads);
            meshBuffers.trisBuffer = trisBuffer;

            return meshBuffers;
        }

        private void SetComputeShaderBuffers(MeshComputeBuffers meshBuffers)
        {
            computeTerrainGen.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            computeTerrainGen.SetBuffer(0, "triangles", meshBuffers.trisBuffer);
            computeTerrainGen.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            computeTerrainGen.SetBuffer(0, "uv", meshBuffers.uvBuffer);
            computeTerrainGen.SetFloat("resolution", vertices.Length);
            computeTerrainGen.SetFloat("maxHeight", maxHeight);
            computeTerrainGen.SetFloat("minHeight", minHeight);
            computeTerrainGen.SetFloat("meshSize", width);
            computeTerrainGen.SetInt("meshLineSize", vertPerSide);
            computeTerrainGen.SetInt("incrementStep", lodIncrementStep);
        }

        private void RetrieveDataFromComputeShader(MeshComputeBuffers meshBuffers)
        {
            meshBuffers.vertBuffer.GetData(vertices);
            meshBuffers.normalBuffer.GetData(normals);
            meshBuffers.uvBuffer.GetData(uv);
            meshBuffers.trisBuffer.GetData(quads);
        }

        private void DisposeBuffersIntoGarbageCollection(MeshComputeBuffers meshBuffers)
        {
            meshBuffers.vertBuffer.Dispose();
            meshBuffers.normalBuffer.Dispose();
            meshBuffers.uvBuffer.Dispose();
            meshBuffers.noiseBuffer.Dispose();
            meshBuffers.trisBuffer.Dispose();
        }

        private void IndexTriangleData()
        {
            triangleIndex = 0;

            for (int i = 0; i < vertPerSide * vertPerSide; i++)
            {
                TriangleSet triangleSet = quads[i];
                IncludeNewTriangles(triangleSet);
            }
        }

        private void IncludeNewTriangles(TriangleSet set)
        {
            if (!(triangleIndex < arrayBoundSize)) return;
            if (set.triangleA == Vector3.zero && set.triangleB == Vector3.zero) return;
            
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

        public void RenderTerrain()
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
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
        }

        private void AssignLerpColors()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            Material sharedMat = renderer.sharedMaterial;
            sharedMat.SetFloat("_MaxHeight", maxHeight);
            sharedMat.SetFloat("_MinHeight", minHeight);
        }

        private void OnGUI()
        {
            if (mesh == null)
            {
                if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
                {
                    PopulateMeshAttributes();
                    AssignLerpColors();
                    BuildTerrain();
                    AssignMeshData();
                    RenderTerrain();
                }
            }

            if (mesh != null)
            {
                if (GUI.Button(new Rect(120, 0, 100, 50), "Regenerate"))
                {
                    PopulateMeshAttributes();
                    AssignLerpColors();
                    BuildTerrain();
                    AssignMeshData();
                    RenderTerrain();
                }
            }
        }
    }
}
