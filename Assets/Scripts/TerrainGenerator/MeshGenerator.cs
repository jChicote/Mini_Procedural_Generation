using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralGeneration.NoiseGeneration;
using System;
using System.Threading;

namespace ProceduralGeneration.MeshGeneration
{
    public class MeshGenerator : MonoBehaviour
    {
        // Insepector accessible fields
        [Header("Mesh Components")]
        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected Mesh mesh;

        // Interfaces
        private INoiseGenerator noiseGenerator;

        // Fields
        private MeshData meshData;
        private float maxHeight = 50;
        private float minimumHeight = 0;
        private int meshLineSize;
        private int lodIncrementStep;
        public const int mapSize = 241;

        private Queue<MeshThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MeshThreadInfo<MeshData>>();

        public float MaxHeight { set { maxHeight = value; } }
        public float MinimumHeight { set { minimumHeight = value; } }
        public INoiseGenerator NoiseGenerator { set { noiseGenerator = value; } }


        private void Awake()
        {
            meshCollider = this.GetComponent<MeshCollider>();
            meshFilter = this.GetComponent<MeshFilter>();
        }

        /// <summary>
        /// Generates the mesh and initial conditions of the terrain
        /// </summary>
        /// <param name="levelOfDetail"></param>
        public void GenerateBaseMesh(int levelOfDetail)
        {
            mesh = new Mesh();
            meshData = new MeshData();
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            meshLineSize = (mapSize - 1) / lodIncrementStep + 1;
            float[] noiseMap = noiseGenerator.CalculateNoise(mapSize, transform.position);

            CalculateMesh(noiseMap);
            AssignMeshData();
            VisualiseMesh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegenerateMesh(int levelOfDetail)
        {
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            meshLineSize = (mapSize - 1) / lodIncrementStep + 1;
            float[] noiseMap = noiseGenerator.CalculateNoise(mapSize, transform.position);

            CalculateMesh(noiseMap);
        }

        /// <summary>
        /// Calculates the mesh vertices and data necessary to define the mesh's characteristics
        /// </summary>
        private void CalculateMesh(float[] noiseMap)
        {
            meshData.SetMeshArrays(meshLineSize);

            for (int index = 0, row = 0; row < mapSize; row += lodIncrementStep)
            {
                for (int col = 0; col < mapSize; index++, col += lodIncrementStep)
                {
                    meshData.vertices[index] = new Vector3(col, CalculateHeight(noiseMap[row * mapSize + col]), row);
                    meshData.normals[index] = -Vector3.forward;
                    meshData.uv[index] = new Vector2(col, row);

                    if (col < mapSize - 1 && row < mapSize - 1)
                    {
                        meshData.AddTriangle(index, index + meshLineSize, index + 1);
                        meshData.AddTriangle(index + 1, index + meshLineSize, index + meshLineSize + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns mesh data items to the mesh object.
        /// </summary>
        /// <param name="mesh"></param>
        public void AssignMeshData()
        {
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.normals = meshData.normals;
            mesh.uv = meshData.uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }

        public void VisualiseMesh()
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public float CalculateHeight(float noiseVal)
        {
            return noiseVal * maxHeight < minimumHeight ? minimumHeight : noiseVal * maxHeight;
        }

        public void RequestMapData(Action<MeshData> callback)
        {
            print("2 encountered");
            ThreadStart threadStart = delegate
            {
                MeshDataThread(callback);
            };

            new Thread(threadStart).Start();
        }

        private void MeshDataThread(Action<MeshData> callback)
        {
            print("4 encountered");
            MeshData meshData = this.meshData;
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new MeshThreadInfo<MeshData>(callback, meshData));
            }
        }

        private void Update()
        {
            if (meshDataThreadInfoQueue.Count > 0)
            {
                print("Encountered THread");
                for(int i = 0; i < meshDataThreadInfoQueue.Count; i++)
                {
                    print("5 encountered");
                    MeshThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        struct MeshThreadInfo<T>
        {
            public Action<T> callback;
            public T parameter;

            public MeshThreadInfo (Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
}
