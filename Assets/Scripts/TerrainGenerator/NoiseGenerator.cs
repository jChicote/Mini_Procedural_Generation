using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.NoiseGeneration
{
    public class NoiseGenerator : MonoBehaviour
    {
        [SerializeField] private float noiseScale;
        [SerializeField] private int stepDetailCount = 3;

        [Range(0.0001f, 1)]
        [SerializeField] private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] private float lacunarity;

        public float[] CalculateNoise(int mapSize, int edgeLength)
        {
            float[] noiseMap = new float[(mapSize + 1) * (mapSize + 1)];
            float amplitude = 1;
            float frequency = 1;
            float noiseValue = 0;

            float scaleX;
            float scaleY;

            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    for (int i = 0; i < stepDetailCount; i++)
                    {
                        scaleX = (float)col / noiseScale * frequency;
                        scaleY = (float)row / noiseScale * frequency;

                        noiseValue += (Mathf.PerlinNoise(scaleX, scaleY) * amplitude);
                        frequency *= lacunarity;
                        amplitude *= persistence;
                    }

                    noiseMap[index] = noiseValue;
                    noiseValue = 0;
                    amplitude = 1;
                    frequency = 1;
                }
            }

            return noiseMap;
        }

        public void NormaliseMap(float[] rawNoise, int mapSize)
        {
            for (int index = 0, row = 0; row <= mapSize; row++)
            {
                for (int col = 0; col <= mapSize; index++, col++)
                {
                    rawNoise[index] = Mathf.InverseLerp(float.MinValue, float.MaxValue, rawNoise[index]);
                    Debug.Log(rawNoise[index]);
                }
            }
        }
    }
}
