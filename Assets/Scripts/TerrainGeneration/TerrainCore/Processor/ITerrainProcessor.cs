using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.TerrainCore.Processor
{
    public interface ITerrainProcessor
    {

        #region - - - - - - Methods - - - - - -

        void ProcessChunkMesh(IChunkMeshAttributes chunkAttributes, float[] noiseData);

        #endregion Methods

    }

}
