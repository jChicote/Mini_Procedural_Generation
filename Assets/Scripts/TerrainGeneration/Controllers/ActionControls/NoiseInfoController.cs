using MiniProceduralGeneration.Generator;
using MiniProceduralGeneration.ScriptableObjects;
using UnityEngine;

namespace MiniProceduralGeneration.Controllers.ActionControls
{

    public interface INoiseInfoController
    {
        void GetNoiseAttributes(INoiseAttributes attributes);
    }

    public class NoiseInfoController : MonoBehaviour, INoiseInfoController
    {

        #region - - - - Fields - - - -

        [SerializeField]
        private TerrainGenerationSettings settings;

        #endregion

        #region - - - - Methods - - - -

        public void GetNoiseAttributes(INoiseAttributes attributes)
        {
            attributes.NoiseScale = settings.noiseAttributes.noiseScale;
            attributes.Persistence = settings.noiseAttributes.persistence;
            attributes.Lacunarity = settings.noiseAttributes.lacunarity;
            attributes.NoiseOctaveCount = settings.noiseAttributes.noiseOctaveCount;
        }

        #endregion

    }

}
