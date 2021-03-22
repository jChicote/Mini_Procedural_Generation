using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.NoiseGeneration
{
    public class NoiseGenerator : MonoBehaviour
    {
        public float[] noiseMap;

        [Range(0.001f, 0.3f)]
        [SerializeField] private float scalar;

        public float[] CalculateNoise(int mapSize, int edgeLength)
        {
            noiseMap = new float[(mapSize + 1) * (mapSize + 1)];

            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    noiseMap[index] = Mathf.PerlinNoise(col * scalar, row * scalar);
                }
            }

            return noiseMap;
        }
    }
}
