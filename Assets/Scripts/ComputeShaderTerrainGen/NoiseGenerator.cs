using UnityEngine;

namespace MiniProceduralGeneration.Generator.Noise
{
    public interface INoiseGenerator
    {
        void GenerateNoiseSeed();
        float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position);
    }

    public class NoiseGenerator : MonoBehaviour, INoiseGenerator
    {
        [SerializeField] private ComputeShader noiseShader;

         struct NoiseComputeBuffers
        {
            public ComputeBuffer noiseBuffer;
            public ComputeBuffer offsetBuffer;
        }

        [Header("Noise Characteristics")]
        [SerializeField] private float noiseScale;
        [SerializeField] private int stepDetailCount = 3;
        private Vector2[] stepOffsets;
        [Range(0.0001f, 1)]
        [SerializeField] private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] private float lacunarity;

        public float[] noiseData;

        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position)
        {
            noiseData = new float[mapSize * mapSize];

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
            int seed = Random.Range(1, 1000000);
            Debug.Log("Seed Created: " + seed);

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
}
