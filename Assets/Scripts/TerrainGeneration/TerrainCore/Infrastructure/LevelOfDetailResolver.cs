using MiniProceduralGeneration.Utility;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure
{

    public class LevelOfDetailResolver : MonoBehaviour
    {

        #region - - - - - - Fields - - - - - -

        public ChunkDimensionsUtility dimensionsUtility;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {

        }

        public float CalculateDistanceToTileCenter()
        {
            return 0f;
        }

        public int CalculateLODTileIncrementStep()
        {
            return 0;
        }

        #endregion Methods
    }

}
