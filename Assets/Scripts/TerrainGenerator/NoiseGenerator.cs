using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.NoiseGeneration
{
    public class NoiseGenerator : MonoBehaviour
    {
        [SerializeField] private float noiseScale;
        [SerializeField] private int stepDetailCount = 3;
        [SerializeField] private int seed;

        [Range(0.0001f, 1)]
        [SerializeField] private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] private float lacunarity;

        private Vector2[] stepOffsets;

        /// <summary>
        /// Generate seed for map
        /// </summary>
        public void GenerateSeed()
        {
            System.Random prng = new System.Random(seed);
            stepOffsets = new Vector2[stepDetailCount];
            for (int i = 0; i < stepDetailCount; i++)
            {
                stepOffsets[i].x = prng.Next(-100000, 100000);
                stepOffsets[i].y = prng.Next(-100000, 100000);
            }
        }

        /// <summary>
        /// Calculates the noisemap for the terrain
        /// </summary>
        public float[] CalculateNoise(int mapSize)
        {
            GenerateSeed();

            float[] noiseMap = new float[(mapSize) * (mapSize)];
            float amplitude = 1;
            float frequency = 1;
            float noiseValue = 0;

            float scaleX;
            float scaleY;

            for (int index = 0, row = 0; row < mapSize; row++)
            {
                for (int col = 0; col < mapSize; index++, col++)
                {
                    for (int i = 0; i < stepDetailCount; i++)
                    {
                        scaleX = (float)col / noiseScale * frequency + stepOffsets[i].x;
                        scaleY = (float)row / noiseScale * frequency + stepOffsets[i].y;

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

            //NormaliseMap(noiseMap, mapSize);

            return noiseMap;
        }

        /// <summary>
        /// Normalises the map values between 0 and 1
        /// </summary>
        /// <param name="rawNoise"></param>
        /// <param name="mapSize"></param>
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
