using System;
using UnityEngine;

namespace MiniProceduralGeneration.ScriptableObjects
{

    [CreateAssetMenu(menuName = "Settings/Terrain Setting")]
    public class TerrainGenerationSettings : ScriptableObject
    {

        #region - - - - Fields - - - -

        public TerrainAttributes terrainAttributes;

        [Space]
        public NoiseAttributes noiseAttributes;

        [Space]
        public MapGridAttributes mapGridAttributes;

        #endregion Fields

    }

    [Serializable]
    public struct TerrainAttributes
    {

        public float maxHeight;
        public float minHeight;
        [Range(0, 6)]
        public int lodIncrementStep;

        // Lower the LOD the higher the resolution
        public int levelOfDetail;
        public int minimumLevelOfDetail;

        // width must contain a base value that follows the "divisibility rules" (add 1 for noise processing).
        public int chunkWidth;

    }

    [Serializable]
    public struct NoiseAttributes
    {

        public float noiseScale;
        public int noiseOctaveCount;
        public float persistence;
        public float lacunarity;

    }

    [Serializable]
    public struct MapGridAttributes
    {
        public int chunkDistance;
    }

}
