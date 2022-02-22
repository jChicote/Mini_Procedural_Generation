using MiniProceduralGeneration.Generator.Entities;
using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    public interface IChunkDimensions
    {
        TerrainChunkDimensions Dimensions { get; set; }
    }

    /// <summary>
    /// A terrain chunk attaches to its own gameobject instance rendering
    /// meshes from the given mesh data.
    /// </summary>
    public class ChunkShell : MonoBehaviour, IChunkShell
    {

        #region - - - - - - Fields - - - - - -

        [SerializeField] protected MeshFilter meshFilter;
        [SerializeField] protected MeshCollider meshCollider;
        [SerializeField] protected Mesh mesh;

        public Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        public int[] meshTriangles;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public TerrainChunkDimensions Dimensions { get; set; }
        public Vector3 PositionWorldSpace { get => transform.position; set => transform.position = value; }
        public Vector3[] Vertices { get => vertices; set => vertices = value; }
        public Vector3[] Normals { get => normals; set => normals = value; }
        public Vector2[] UVs { get => uv; set => uv = value; }
        public int[] Triangles
        {
            get => meshTriangles; set => meshTriangles = value;
        }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions)
        {
            vertices = new Vector3[chunkDimensions.SquaredVertexSide];
            normals = new Vector3[chunkDimensions.SquaredVertexSide];
            uv = new Vector2[chunkDimensions.SquaredVertexSide];
            meshTriangles = new int[(chunkDimensions.VertexPerSide - 1) * (chunkDimensions.VertexPerSide - 1) * 6];

            this.Dimensions = chunkDimensions;
        }

        public void BuildMesh()
        {
            mesh = new Mesh();

            print("Verticies: " + vertices.Length);

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
            Destroy(gameObject, 1);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(PositionWorldSpace, 2);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Vertices[Vertices.Length - 1], 2);
        }

        #endregion

    }

}
