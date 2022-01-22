using MiniProceduralGeneration.Generator.Entities;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.MeshWork
{
    public interface ITerrainChunk : IChunkMeshAttributes
    {
        Vector3 PositionWorldSpace { get; set; }
        void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions);
        void BuildMesh();
        void OnDestroyChunk();
    }

    /// <summary>
    /// Interfaces for accessing terrain chunks properties for modifying
    /// mesh information.
    /// </summary>
    public interface IChunkMeshAttributes
    {
        Vector3[] Vertices { get; set; }
        Vector3[] Normals { get; set; }
        Vector2[] UVs { get; set; }
        int[] Triangles { get; set; }
    }

    /// <summary>
    /// A terrain chunk attaches to its own gameobject instance rendering
    /// meshes from the given mesh data.
    /// </summary>
    public class ChunkBody : MonoBehaviour, ITerrainChunk
    {
        // Fields
        [Header("Mesh Components")]
        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected Mesh mesh;

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private int[] meshTriangles;

        // Properties
        public Vector3 PositionWorldSpace { get => transform.position; set => transform.position = value; }
        public Vector3[] Vertices { get => vertices; set => vertices = value; }
        public Vector3[] Normals { get => normals; set => normals = value; }
        public Vector2[] UVs { get => uv; set => uv = value; }
        public int[] Triangles { get => meshTriangles; set => meshTriangles = value; }

        public void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions)
        {
            vertices = new Vector3[chunkDimensions.SquaredVertexSide];
            normals = new Vector3[chunkDimensions.SquaredVertexSide];
            uv = new Vector2[chunkDimensions.SquaredVertexSide];
            meshTriangles = new int[(chunkDimensions.VertexPerSide - 1) * (chunkDimensions.VertexPerSide - 1) * 6];
        }

        public void BuildMesh()
        {
            mesh = new Mesh();

            AssignDataToMesh();
            RenderTerrain();
        }

        /// <summary>
        /// Assigns mesh data items to the mesh object and recalculates mesh for render.
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
        /// Pushes prepared mesh to the screen
        /// </summary>
        public void RenderTerrain() // Renders mesh
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void OnDestroyChunk()
        {
            //print("Chunk is being destroyed");
            Destroy(gameObject, 1);
        }
    }
}
