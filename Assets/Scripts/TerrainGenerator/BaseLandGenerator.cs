using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.TerrainGeneration
{
    public class BaseLandGenerator : MonoBehaviour
    {
        // Insepector accessible fields
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private NoiseGenerator noiseGenerator;

        [SerializeField] private float width;
        [SerializeField] private float height;

        [SerializeField] private int mapSize;
        [SerializeField] private int edgeLength;

        // Fields
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private int[] triangles;

        private void Start()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
            meshCollider = this.GetComponent<MeshCollider>();
            vertices = new Vector3[4];
            normals = new Vector3[4];
            uv = new Vector2[4];

            //GenerateBasicQuad();
            GenerateMap();
        }

        /// <summary>
        /// Generates basic quad for testing purposes
        /// </summary>
        public void GenerateBasicQuad()
        {
            Mesh mesh = new Mesh();
            int size = edgeLength * mapSize;

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

            vertices = new Vector3[(mapSize + 1) * (mapSize + 1)];
            normals = new Vector3[(mapSize + 1) * (mapSize + 1)];
            uv = new Vector2[(mapSize + 1) * (mapSize + 1)];
            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    vertices[index] = new Vector3(col * edgeLength, 0, row * edgeLength);
                    normals[index] = -Vector3.forward;
                    uv[index] = new Vector2(col, row);
                }
            }

            DetermineMeshTriangles();
            AssignMesh(mesh);
        }

        public void DetermineMeshTriangles()
        {
            triangles = new int[mapSize * mapSize * 6];

            for (int row = 0, vi = 0, ti = 0; row < mapSize; row++, vi++)
            {
                for (int col = 0; col < mapSize; col++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + mapSize + 1;
                    triangles[ti + 5] = vi + mapSize + 2;
                }
            }
        }

        public void AssignMesh(Mesh mesh)
        {
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
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
