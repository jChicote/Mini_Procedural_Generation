using MiniProceduralGeneration.Chunk;
using System;
using System.Collections;

namespace MiniProceduralGeneration.TerrainCore.Processor
{
    public interface IMeshTerrainProcessor
    {

        #region - - - - - - Methods - - - - - -

        IEnumerator ProcessChunkMesh(IChunkShell chunk, float[] noiseData, Action chunkAction);

        void ProcessChunkMesh(IChunkShell chunk, float[] noiseData);

        #endregion Methods

    }

}
