namespace MiniProceduralGeneration.Utility
{

    public class MapArrayUtility
    {

        #region - - - - - - Methods - - - - - -

        public static int GetIndexFromRowAndCol(int mapEdgeSize, int row, int col)
        {
            return row * mapEdgeSize + col;
        }

        #endregion Methods

    }

}
