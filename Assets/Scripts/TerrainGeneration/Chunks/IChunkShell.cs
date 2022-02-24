using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using MiniProceduralGeneration.TerrainCore;
using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    public interface IChunkShell : IChunkMeshAttributes, IChunkDimensions
    {

        #region - - - - - - Properties - - - - - -

        Vector3 PositionWorldSpace { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void InitChunkShell(TerrainChunkDimensions chunkDimensions, ITerrainAttributes terrainAttributes, ISeedGenerator seedGenerator, INoiseOffsetGenerator offsetGenerator);
        void BuildMesh();
        void OnDestroyChunk();

        #endregion Methods

    }

}
