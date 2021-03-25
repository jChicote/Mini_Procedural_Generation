using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralGeneration.NoiseGeneration;

namespace ProceduralGeneration.TerrainGeneration
{
    public class BaseLandGenerator : MonoBehaviour
    {
        // Insepector accessible fields
        [Header("Mesh Components")]
        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected NoiseGenerator noiseGenerator;

        [Header("Map Details")]
        [SerializeField] protected float maxHeight;
        [SerializeField] protected int mapSize;
        [SerializeField] protected float edgeLength;
        [SerializeField] protected int seaLevel;

        [Header("Regenerate Map")]
        [SerializeField] protected bool regenerate;

        // Fields
        protected Vector3[] vertices;
        protected Vector3[] normals;
        protected Vector2[] uv;
        protected int[] triangles;
        private float pointHeight;

        private void Start()
        {
            meshCollider = this.GetComponent<MeshCollider>();
            vertices = new Vector3[4];
            normals = new Vector3[4];
            uv = new Vector2[4];

            GenerateMap();
        }

        private void Update()
        {
            if (!regenerate) return;

            GenerateMap();
        }

        /// <summary>
        /// Generates basic quad for testing purposes
        /// </summary>
        public void GenerateBasicQuad()
        {
            Mesh mesh = new Mesh();
            int size = (int)edgeLength * mapSize;

            vertices = new Vector3[4]
            {
            new Vector3(0, 0, 0),
            new Vector3(size, 0, 0),
            new Vector3(0, 0, size),
            new Vector3(size, 0, size)
            };
            mesh.vertices = vertices;

            // The tris is the triangular geometry calculated within the quad
            int[] tris = new int[6]
            {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
            };
            mesh.triangles = tris;

            normals = new Vector3[4]
            {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
            };
            mesh.normals = normals;

            uv = new Vector2[4]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
            };
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public virtual void GenerateMap()
        {
            Mesh mesh = new Mesh();
            float[] noiseMap = noiseGenerator.CalculateNoise(mapSize);

            GenerateMeshData(noiseMap);
            DetermineMeshTriangles();
            AssignMeshData(mesh);
            regenerate = false;
        }

        /// <summary>
        /// Generates the mesh data for the terrain.
        /// </summary>
        /// <param name="mesh">Mesh object for the data.</param>
        /// <param name="noiseData">Noise data for the mesh.</param>
        public virtual void GenerateMeshData(float[] noiseData)
        {
            vertices = new Vector3[(mapSize + 1) * (mapSize + 1)];
            normals = new Vector3[(mapSize + 1) * (mapSize + 1)];
            uv = new Vector2[(mapSize + 1) * (mapSize + 1)];
            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    vertices[index] = new Vector3(col * edgeLength, CalculateHeight(noiseData[index]), row * edgeLength);
                    normals[index] = -Vector3.forward;
                    uv[index] = new Vector2(col, row);
                }
            }
        }

        /// <summary>
        /// Prepares array of triangles corresponding to triangle indices within mesh quad.
        /// </summary>
        public virtual void DetermineMeshTriangles()
        {
            /*triangles = new int[mapSize * mapSize * 6];

            for (int row = 0, vi = 0, ti = 0; row < mapSize; row++, vi++)
            {
                for (int col = 0; col < mapSize; col++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + mapSize + 1;
                    triangles[ti + 5] = vi + mapSize + 2;
                }
            }*/
        }

        /// <summary>
        /// Assigns data to mesh and prepares mesh for renderers and collider.
        /// </summary>
        /// <param name="mesh">Mesh object for data insertion</param>
        public virtual void AssignMeshData(Mesh mesh)
        {
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public float CalculateHeight(float noiseVal)
        {
            pointHeight = noiseVal * maxHeight;
            return pointHeight < seaLevel ? seaLevel : pointHeight;
        }

        /*
        private void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0) return;

            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        } */
    }
}
