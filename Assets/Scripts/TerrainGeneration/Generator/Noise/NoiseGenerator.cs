using MiniProceduralGeneration.Controllers.ActionControls;
using MiniProceduralGeneration.Generator.Processor;
using MiniProceduralGeneration.Generator.Seed;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator
{

    public interface INoiseOffsetsInvoker
    {

        void CreateStepOffsets();

    }

    public interface INoiseGenerator : INoiseOffsetsInvoker
    {

        float[] NoiseData { get; }
        float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position);

    }

    public interface INoiseAttributes
    {

        #region - - - - Properties - - - -

        float NoiseScale { get; set; }
        float Persistence { get; set; }
        float Lacunarity { get; set; }
        int NoiseOctaveCount { get; set; }
        Vector2[] StepOffsets { get; }

        #endregion Properties

    }

    /// <summary>
    /// A class to generate noise for terrain processing.
    /// </summary>
    public class NoiseGenerator : GameHandler, INoiseGenerator, INoiseAttributes
    {

        #region - - - - Fields - - - -

        private INoiseProcessor noiseProcessor;
        private INoiseInfoController infoController;
        private ISeedGenerator seedGenerator;

        private Vector2[] octavePositionOffsets;
        private float[] noiseData;

        #endregion Fields

        #region - - - - Properties - - - -

        public float[] NoiseData => noiseData;
        public float NoiseScale { get; set; }
        public float Persistence { get; set; }
        public float Lacunarity { get; set; }
        public int NoiseOctaveCount { get; set; }
        public Vector2[] StepOffsets => octavePositionOffsets;

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void Awake()
        {
            noiseProcessor = this.GetComponent<INoiseProcessor>();
            seedGenerator = this.GetComponent<ISeedGenerator>();

            infoController = FindObjectOfType<NoiseInfoController>(); // Can be Better
            if (infoController != null)
                infoController.GetNoiseAttributes(this);
        }

        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 samplePosition)
        {
            noiseData = new float[25 * 25];
            noiseProcessor.ProcessNoiseData(noiseData, 25, samplePosition);

            return noiseData;
        }

        /// <summary>
        /// Creates position offsets against sample position for noise data complexity.
        /// </summary>
        public void CreateStepOffsets()
        {
            System.Random psuedoRandNumbGenerator = new System.Random(seedGenerator.Seed);
            octavePositionOffsets = new Vector2[NoiseOctaveCount];
            for (int i = 0; i < NoiseOctaveCount; i++)
            {
                octavePositionOffsets[i].x = psuedoRandNumbGenerator.Next(-100000, 100000);
                octavePositionOffsets[i].y = psuedoRandNumbGenerator.Next(-100000, 100000);
                print(octavePositionOffsets[i].x + ", " + octavePositionOffsets[i].y);
            }
        }

        #endregion Methods

    }

}
