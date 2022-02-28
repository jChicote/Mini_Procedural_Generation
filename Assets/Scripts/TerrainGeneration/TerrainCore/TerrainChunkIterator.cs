using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore
{

    public class TerrainChunkIterator : MonoBehaviour, ITerrainChunkIterator
    {

        #region - - - - Fields - - - -

        private ChunkDimensionsResolver chunkResolver;
        private TerrainChunkDimensions chunkDimensions;

        private INoiseOffsetGenerator offsetGenerator;
        private ITerrainAttributes attributes;

        #endregion Fields

        #region - - - - Constructors - - - -

        public void StartTerrainRunnerAction(ITerrainAttributes attributes)
        {
            this.attributes = attributes;

            chunkResolver = this.GetComponent<ChunkDimensionsResolver>();
            offsetGenerator = this.GetComponent<INoiseOffsetGenerator>();
        }

        #endregion Constructors

        #region - - - - Methods - - - -

        public void IterateThroughChunkArraySelection(IChunkShell[] chunks)
        {
            foreach (IChunkShell chunk in chunks)
            {
                ProcessChunk(chunk, false);
            }
        }

        public void ProcessChunk(IChunkShell chunk, bool forceUpdate)
        {
            chunkDimensions = chunkResolver.GetChunkDimensions(chunk.PositionWorldSpace);

            if (!forceUpdate && chunk.Dimensions != null)
                if (chunk.Dimensions.LevelOfDetail == chunkDimensions.LevelOfDetail)
                    return;

            if (forceUpdate)
                chunk.DisableMeshRenderer();

            var seedGenerator = this.GetComponent<ISeedGenerator>();

            chunk.InitChunkShell(chunkDimensions, attributes, seedGenerator, offsetGenerator);
            chunk.BuildMesh();
        }

    }

    #endregion Methods

}

