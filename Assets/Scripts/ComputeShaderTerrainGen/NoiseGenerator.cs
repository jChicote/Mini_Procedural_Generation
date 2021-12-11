using UnityEngine;
using MiniProceduralGeneration.Generator.Processor;

namespace MiniProceduralGeneration.Generator
{
    public interface INoiseGenerator
    {
        bool HasCreatedSeed { get; }
        void GenerateSeed();
        float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position);
    }

    public interface INoiseCharacteristics
    {
        float NoiseScale { get; }
        float Persistence { get; }
        float Lacunarity { get; }
        int StepDetailCount { get; }
        Vector2[] StepOffsets { get; }
    }

    public class NoiseGenerator : MonoBehaviour, INoiseGenerator, INoiseCharacteristics
    {
        // Fields
        private INoiseProcessor noiseProcessor;

        [Header("Noise Characteristics")]
        [SerializeField] 
        private float noiseScale;
        [SerializeField] 
        private int stepDetailCount = 3;
        private Vector2[] stepOffsets;

        [Range(0.0001f, 1)]
        [SerializeField] 
        private float persistence = 0.5f;
        [Range(0.0001f, 2)]
        [SerializeField] 
        private float lacunarity;

        private float[] noiseData;
        private int seed;

        // Properties
        public bool HasCreatedSeed => seed != 0;
        public float NoiseScale => noiseScale;
        public float Persistence => persistence;
        public float Lacunarity => lacunarity;
        public int StepDetailCount => stepDetailCount;
        public Vector2[] StepOffsets => stepOffsets;

        private void Awake()
        {
            noiseProcessor = this.GetComponent<INoiseProcessor>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapSize"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 samplePosition)
        {
            noiseData = new float[mapSize * mapSize];
            noiseProcessor.ProcessNoiseData(noiseData, mapSize, samplePosition); 

            return noiseData;
        }

        public void GenerateSeed() // Might need to refactor
        {
            seed = Random.Range(1, 1000000);
            Debug.Log("Seed Created: " + seed);

            CreateStepOffsets();
        }

        /// <summary>
        /// 
        /// </summary>
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
    }
}
