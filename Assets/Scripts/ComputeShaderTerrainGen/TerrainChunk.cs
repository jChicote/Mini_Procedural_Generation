using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.MeshWork
{
    public interface ITerrainChunk : ITerrainMeshAttributeModifier
    {
        Vector3 PositionWorldSpace { get; }
        void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions);
        void BuildMesh();
    }

    public interface ITerrainMeshAttributeModifier
    {
        Vector3[] Vertices { get; set; }
        Vector3[] Normals { get; set; }
        Vector2[] UVs { get; set; }
        QuadSet[] Quads { get; set; }
        int[] Triangles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TerrainChunk : MonoBehaviour, ITerrainChunk
    {
        // Fields
        [Header("Mesh Components")]
        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected Mesh mesh;

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private QuadSet[] quads;
        private TerrainChunkDimensions chunkDimensions;
        private int[] meshTriangles;

        private int triangleIndex = 0;
        private int arrayBoundSize = 0;

        // Properties
        public Vector3 PositionWorldSpace => transform.position;
        public Vector3[] Vertices { get => vertices; set => vertices = value; }
        public Vector3[] Normals { get => normals; set => normals = value; }
        public Vector2[] UVs { get => uv; set => uv =  value; }
        public QuadSet[] Quads { get => quads; set => quads = value; }
        public int[] Triangles { get => meshTriangles; set => meshTriangles = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkDimensions"></param>
        public void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions)
        {
            this.chunkDimensions = chunkDimensions;

            vertices = new Vector3[chunkDimensions.squaredVertexSide];
            normals = new Vector3[chunkDimensions.squaredVertexSide];
            uv = new Vector2[chunkDimensions.squaredVertexSide];
            quads = new QuadSet[chunkDimensions.squaredVertexSide];
            meshTriangles = new int[(chunkDimensions.vertexPerSide - 1) * (chunkDimensions.vertexPerSide - 1) * 6];
            arrayBoundSize = (chunkDimensions.vertexPerSide - 1) * (chunkDimensions.vertexPerSide - 1) * 6;
        }

        public void BuildMesh()
        {
            mesh = new Mesh();

            //ProcessTrianglesInQuadArray();
            AssignDataToMesh();
            RenderTerrain();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessTrianglesInQuadArray()
        {
            triangleIndex = 0; // Tracks current triangle index

            for (int i = 0; i < chunkDimensions.squaredVertexSide; i++)
            {
                if ((triangleIndex < arrayBoundSize))
                {
                    QuadSet triangleSet = quads[i];
                    ProcessQuadTriangles(triangleSet);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        private void ProcessQuadTriangles(QuadSet set)
        {
            if (set.triangleA == Vector3.zero && set.triangleB == Vector3.zero) return;

            // first triangle
            AddToMeshTriangles(set.triangleA);

            // second triangle
            AddToMeshTriangles(set.triangleB);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="triangle"></param>
        private void AddToMeshTriangles(Vector3 triangle)
        {
            meshTriangles[triangleIndex] = (int)triangle.x;
            meshTriangles[triangleIndex + 1] = (int)triangle.y;
            meshTriangles[triangleIndex + 2] = (int)triangle.z;
            triangleIndex += 3;
        }

        /// <summary>
        /// Assigns mesh data items to the mesh object.
        /// </summary>
        /// <param name="mesh"></param>
        public void AssignDataToMesh()
        {
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;
            mesh.normals = normals;
            mesh.uv = uv;

            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RenderTerrain() // Renders mesh
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }

    /// <summary>
    /// Contains set of triangles containing index positions within mesh array.
    /// </summary>
    public struct QuadSet
    {
        public Vector3 triangleA;
        public Vector3 triangleB;
    }
}
