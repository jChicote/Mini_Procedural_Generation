using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.NoiseGeneration
{
    public class NoiseGenerator : MonoBehaviour
    {
        public float[] noiseMap;

        public float[] CalculateNoise(int mapSize, int edgeLength)
        {
            noiseMap = new float[(mapSize + 1) * (mapSize + 1)];

            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    noiseMap[index] = Mathf.PerlinNoise(col * edgeLength, row * edgeLength);
                }
            }

            return noiseMap;
        }
    }
}
