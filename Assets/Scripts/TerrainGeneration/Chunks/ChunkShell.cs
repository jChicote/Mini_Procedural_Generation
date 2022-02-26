using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using MiniProceduralGeneration.TerrainCore;
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
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected Mesh mesh;

        public Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private int[] meshTriangles;

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

        public virtual void InitChunkShell(TerrainChunkDimensions chunkDimensions, ITerrainAttributes terrainAttributes, ISeedGenerator seedGenerator, INoiseOffsetGenerator offsetGenerator)
        {
            vertices = new Vector3[chunkDimensions.SquaredVertexSide];
            normals = new Vector3[chunkDimensions.SquaredVertexSide];
            uv = new Vector2[chunkDimensions.SquaredVertexSide];
            meshTriangles = new int[(chunkDimensions.VertexPerSide - 1) * (chunkDimensions.VertexPerSide - 1) * 6];

            this.Dimensions = chunkDimensions;
        }

        public virtual void BuildMesh()
        {
            mesh = new Mesh();

            AssignDataToMesh();
            RenderTerrain();
        }

        public void DisableMeshRenderer()
            => meshRenderer.enabled = false;

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

        public void RenderTerrain() // Renders mesh
        {
            meshFilter.mesh = mesh;
            //meshCollider.sharedMesh = mesh;
            SetMeshCollider(mesh);
        }

        public void SetMeshCollider(Mesh mesh)
        {
            if (Dimensions.LevelOfDetail == Dimensions.MinimumLevelOfDetail)
            {
                meshCollider.enabled = false;
                return;
            }

            //meshCollider.enabled = Dimensions.LevelOfDetail == Dimensions.MinimumLevelOfDetail ? false : true;
            //meshCollider.sharedMesh = meshCollider.enabled ? mesh : null;


            meshCollider.enabled = true;
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
        }

        #endregion

    }

}
