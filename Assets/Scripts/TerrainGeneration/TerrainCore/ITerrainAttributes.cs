namespace MiniProceduralGeneration.TerrainCore
{
    public interface ITerrainAttributes
    {

        #region - - - - - - Properties - - - - - -

        float AbsoluteHeight { get; set; }
        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        int ActualChunkSize { get; set; }
        int RenderChunkSize { get; }
        int LODIncrementStep { get; set; }
        int VertexPerSide { get; }
        float LevelOfDetail { get; set; }

        #endregion Properties

    }

}
