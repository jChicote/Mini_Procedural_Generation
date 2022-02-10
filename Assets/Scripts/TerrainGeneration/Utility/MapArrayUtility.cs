namespace MiniProceduralGeneration.Generator.Utility
{
    public class MapArrayUtility
    {
        public static int GetIndexFromRowAndCol(int mapEdgeSize, int row, int col)
        {
            return row * mapEdgeSize + col;
        }
    }
}
