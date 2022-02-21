using MiniProceduralGeneration.Generator.Entities;
using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    public interface IChunkShell : IChunkMeshAttributes, IChunkDimensions
    {

        #region - - - - - - Properties - - - - - -

        Vector3 PositionWorldSpace { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void InitialiseMeshArrays(TerrainChunkDimensions chunkDimensions);
        void BuildMesh();
        void OnDestroyChunk();

        #endregion Methods

    }

}
