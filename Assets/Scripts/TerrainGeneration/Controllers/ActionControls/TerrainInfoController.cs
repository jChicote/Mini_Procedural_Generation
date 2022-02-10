using MiniProceduralGeneration.Generator;
using MiniProceduralGeneration.ScriptableObjects;
using UnityEngine;

namespace MiniProceduralGeneration.Controllers.ActionControls
{

    public interface ITerrainInfoController
    {
        void GetTerrainAttributes(ITerrainAttributes attributes);
    }

    public class TerrainInfoController : MonoBehaviour, ITerrainInfoController
    {
        #region - - - - Fields - - - -

        [SerializeField]
        private TerrainGenerationSettings settings;

        #endregion Fields

        #region - - - - Methods - - - -

        public void GetTerrainAttributes(ITerrainAttributes attributes)
        {
            attributes.MaxHeight = settings.terrainAttributes.maxHeight;
            attributes.MinHeight = settings.terrainAttributes.minHeight;
            attributes.LODIncrementStep = settings.terrainAttributes.lodIncrementStep;
            attributes.LevelOfDetail = settings.terrainAttributes.levelOfDetail;
            attributes.ActualChunkSize = settings.terrainAttributes.chunkWidth;
        }

        #endregion Methods

    }

}