using MiniProceduralGeneration.Generator.Processor;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator
{
    public interface INoiseGenerator
    {
        bool HasCreatedSeed { get; }
        float[] NoiseData { get; }
        void GenerateSeed();
        float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position);
    }

    public interface INoiseAttributes
    {
        float NoiseScale { get; set; }
        float Persistence { get; set; }
        float Lacunarity { get; set; }
        float NoiseOctaveCount { get; set; }
        Vector2[] StepOffsets { get; }
    }

    /// <summary>
    /// A class to generate noise for terrain processing.
    /// </summary>
    public class NoiseGenerator : GameHandler, INoiseGenerator, INoiseAttributes
    {
        // Fields
        private INoiseProcessor noiseProcessor;

        [Header("Noise Characteristics")]
        [SerializeField] private float noiseScale;
        [SerializeField] private int noiseOctaveCount = 3;

        [Range(0.0001f, 1)]
        [SerializeField] private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] private float lacunarity;

        private Vector2[] octavePositionOffsets;
        private float[] noiseData;
        private int seed;

        // Properties
        public bool HasCreatedSeed => seed != 0;
        public float[] NoiseData => noiseData;
        public float NoiseScale { get => noiseScale; set => noiseScale = value; }
        public float Persistence { get => persistence; set => persistence = value; }
        public float Lacunarity { get => lacunarity; set => lacunarity = value; }
        public float NoiseOctaveCount { get => noiseOctaveCount; set => noiseOctaveCount = (int)value; }
        public Vector2[] StepOffsets => octavePositionOffsets;

        public void Awake()
        {
            noiseProcessor = this.GetComponent<INoiseProcessor>();
        }

        public void GenerateSeed()
        {
            seed = Random.Range(1, 1000000);
            print("Seed Created: " + seed);

            CreateStepOffsets();
        }

        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 samplePosition)
        {
            noiseData = new float[mapSize * mapSize];
            noiseProcessor.ProcessNoiseData(noiseData, mapSize, samplePosition);

            return noiseData;
        }

        /// <summary>
        /// Creates position offsets against sample position for noise data complexity.
        /// </summary>
        private void CreateStepOffsets()
        {
            System.Random psuedoRandNumbGenerator = new System.Random(seed);
            octavePositionOffsets = new Vector2[noiseOctaveCount];
            for (int i = 0; i < noiseOctaveCount; i++)
            {
                octavePositionOffsets[i].x = psuedoRandNumbGenerator.Next(-100000, 100000);
                octavePositionOffsets[i].y = psuedoRandNumbGenerator.Next(-100000, 100000);
            }
        }
    }
}
