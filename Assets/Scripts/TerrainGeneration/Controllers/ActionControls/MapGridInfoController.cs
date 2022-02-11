using MiniProceduralGeneration.MapGrid;
using MiniProceduralGeneration.ScriptableObjects;
using UnityEngine;

namespace MiniProceduralGeneration.Controllers
{

    public interface IMapGridInfoController
    {

        #region - - - - - - Methods - - - - - -

        void GetMapGridAttributes(IMapGridAttributes destination);

        #endregion Methods

    }

    public class MapGridInfoController : MonoBehaviour, IMapGridInfoController
    {

        #region - - - - Fields - - - -

        public TerrainGenerationSettings settings;

        #endregion

        #region - - - - Methods - - - -

        public void GetMapGridAttributes(IMapGridAttributes destination)
        {
            destination.ChunkDistance = settings.mapGridAttributes.chunkDistance;
        }

        #endregion Methods

    }

}
