using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.ScriptableObjects;
using UnityEngine;

namespace MiniProceduralGeneration.Controllers
{

    public interface IMapGridInfoController
    {
        void GetMapGridAttributes(IMapGridAttributes destination);
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
