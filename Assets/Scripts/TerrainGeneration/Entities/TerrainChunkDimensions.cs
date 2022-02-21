namespace MiniProceduralGeneration.Generator.Entities
{

    [System.Serializable]
    public class TerrainChunkDimensions
    {

        #region ------ Properties ------

        public int LevelOfDetail { get; set; }

        public int VertexPerSide { get; set; }

        public int SquaredVertexSide { get; set; }

        #endregion Properties

    }

}
