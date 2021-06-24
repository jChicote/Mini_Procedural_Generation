using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaderTerrainGeneration
{
    public interface INoiseGenerator
    {
        void GenerateNoiseSeed();
        float[] CalculateNoise(int mapSize, Vector3 position);
    }

    public class NoiseGenerator : MonoBehaviour, INoiseGenerator
    {
        [SerializeField] private ComputeShader noiseShader;
        [SerializeField] private float noiseScale;
        [SerializeField] private int stepDetailCount = 3;
        private Vector2[] stepOffsets;
        public float[] noiseData;
        [Range(0.0001f, 1)]
        [SerializeField] private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] private float lacunarity;

        [SerializeField] private int seed;

        public float[] CalculateNoise(int mapSize, Vector3 position)
        {
            noiseData = new float[mapSize * mapSize];
            /*float amplitude = 1;
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
                        scaleX = (float)col / noiseScale * frequency + stepOffsets[i].x + position.x;
                        scaleY = (float)row / noiseScale * frequency + stepOffsets[i].y + position.z;

                        noiseValue += (Mathf.PerlinNoise(scaleX, scaleY) * amplitude);
                        frequency *= lacunarity;
                        amplitude *= persistence;
                    }

                    noiseData[index] = noiseValue;
                    noiseValue = 0;
                    amplitude = 1;
                    frequency = 1;
                }
            }*/

            NoiseComputeBuffers computeBuffers = CreateNoiseComputeBuffers();
            SetComputeShaderData(computeBuffers, position, mapSize);
            noiseShader.Dispatch(0, noiseData.Length / 10, 1, 1);

            GetDataFromComputeShader(computeBuffers);

            // Manual Garbage collection of Buffers from memory
            DisposeBuffers(computeBuffers);

            return noiseData;
        }

        public void GenerateNoiseSeed()
        {
            System.Random prng = new System.Random(seed);
            stepOffsets = new Vector2[stepDetailCount];
            for (int i = 0; i < stepDetailCount; i++)
            {
                stepOffsets[i].x = prng.Next(-100000, 100000);
                stepOffsets[i].y = prng.Next(-100000, 100000);
            }
        }

        private NoiseComputeBuffers CreateNoiseComputeBuffers()
        {
            NoiseComputeBuffers computeBuffers = new NoiseComputeBuffers();

            int noiseSize = sizeof(float);
            ComputeBuffer noiseBuffer = new ComputeBuffer(noiseData.Length, noiseSize);
            noiseBuffer.SetData(noiseData);
            computeBuffers.noiseBuffer = noiseBuffer;

            int offsetSize = sizeof(float) * 2;
            ComputeBuffer offsetBuffer = new ComputeBuffer(stepOffsets.Length, offsetSize);
            offsetBuffer.SetData(stepOffsets);
            computeBuffers.offsetBuffer = offsetBuffer;

            return computeBuffers;
        }

        private void SetComputeShaderData(NoiseComputeBuffers computeBuffers, Vector3 startPosition, int mapSize)
        {
            noiseShader.SetBuffer(0, "noise", computeBuffers.noiseBuffer);
            noiseShader.SetBuffer(0, "stepOffsets", computeBuffers.offsetBuffer);
            noiseShader.SetVector("startPosition", startPosition);
            noiseShader.SetFloat("noiseScale", noiseScale);
            noiseShader.SetFloat("persistence", persistence);
            noiseShader.SetFloat("lacunarity", lacunarity);
            noiseShader.SetInt("mapDimension", mapSize);
            noiseShader.SetInt("stepDetailCount", stepDetailCount);
        }

        private void GetDataFromComputeShader(NoiseComputeBuffers computeBuffers)
        {
            computeBuffers.noiseBuffer.GetData(noiseData);
        }

        private void DisposeBuffers(NoiseComputeBuffers computeBuffers)
        {
            computeBuffers.noiseBuffer.Dispose();
            computeBuffers.offsetBuffer.Dispose();
        }
    }

    public struct NoiseComputeBuffers
    {
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer offsetBuffer;
    }
}
