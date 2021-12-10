using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.MeshWork
{
    public interface ITerrainChunk : ITerrainMeshAttributeModifier
    {
        void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions);
    }

    public interface ITerrainMeshAttributeModifier
    {
        Vector3[] Vertices { get; set; }
        Vector3[] Normals { get; set; }
        Vector2[] UVs { get; set; }
        QuadSet[] Quads { get; set; }
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
        public Vector3[] Vertices { get => vertices; set => vertices = value; }
        public Vector3[] Normals { get => normals; set => normals = value; }
        public Vector2[] UVs { get => uv; set => uv =  value; }
        public QuadSet[] Quads { get => quads; set => quads = value; }

        //
        //
        //
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

        //
        //
        //
        public void BuildMesh()
        {
            mesh = new Mesh();

            IndexTriangleData();
            AssignMeshData();
            RenderTerrain();
        }

        //
        //
        //
        private void IndexTriangleData() // Renders mesh
        {
            triangleIndex = 0;

            for (int i = 0; i < chunkDimensions.squaredVertexSide; i++)
            {
                QuadSet triangleSet = quads[i];
                IncludeNewTriangles(triangleSet);
            }
        }

        //
        //
        //
        private void IncludeNewTriangles(QuadSet set) // Renders mesh
        {
            if (!(triangleIndex < arrayBoundSize)) return;
            if (set.triangleA == Vector3.zero && set.triangleB == Vector3.zero) return;

            // first triangle
            AddTriangle(set.triangleA);

            // second triangle
            AddTriangle(set.triangleB);
        }
        
        //
        //
        //
        private void AddTriangle(Vector3 triangle) // Renders mesh
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
        public void AssignMeshData()
        {
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;
            //mesh.normals = normals;
            mesh.uv = uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }

        //
        //
        //
        public void RenderTerrain() // Renders mesh
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
