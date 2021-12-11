using UnityEngine;

namespace MiniProceduralGeneration.Generator.Noise
{
    public interface INoiseGenerator
    {
        bool HasCreatedSeed { get; }
        void GenerateSeed();
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
        private int seed;

        public bool HasCreatedSeed { get => seed != 0;  }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapSize"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position)
        {
            noiseData = new float[mapSize * mapSize];
            NoiseComputeBuffers computeBuffers = CreateNoiseComputeBuffers();
            
            // Produce noise data through compute shader
            SetComputeShaderData(computeBuffers, position, mapSize);
            noiseShader.Dispatch(0, noiseData.Length / 10, 1, 1);
            GetDataFromComputeShader(computeBuffers);

            // Manual Garbage collection of Buffers from memory
            DisposeBuffers(computeBuffers);

            return noiseData;
        }

        public void GenerateSeed() // Might need to refactor
        {
            seed = Random.Range(1, 1000000);
            Debug.Log("Seed Created: " + seed);

            CreateStepOffsets();
        }

        private void CreateStepOffsets()
        {
            System.Random psuedoRandNumbGenerator = new System.Random(seed);
            stepOffsets = new Vector2[stepDetailCount];
            for (int i = 0; i < stepDetailCount; i++)
            {
                stepOffsets[i].x = psuedoRandNumbGenerator.Next(-100000, 100000);
                stepOffsets[i].y = psuedoRandNumbGenerator.Next(-100000, 100000);
            }
        }

        private NoiseComputeBuffers CreateNoiseComputeBuffers()
        {
            NoiseComputeBuffers computeBuffers = new NoiseComputeBuffers();
            
            computeBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float)); ;
            computeBuffers.noiseBuffer.SetData(noiseData);
            
            computeBuffers.offsetBuffer = new ComputeBuffer(stepOffsets.Length, sizeof(float) * 2); ;
            computeBuffers.offsetBuffer.SetData(stepOffsets);

            return computeBuffers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="computeBuffers"></param>
        /// <param name="startPosition"></param>
        /// <param name="mapSize"></param>
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
