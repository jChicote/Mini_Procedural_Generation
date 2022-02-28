using MiniProceduralGeneration.Controllers.ActionControls;
using MiniProceduralGeneration.Handler;
using MiniProceduralGeneration.Noise.Processor;
using UnityEngine;

namespace MiniProceduralGeneration.Noise
{

    //public interface INoiseOffsetsInvoker
    //{

    //    #region - - - - - - Methods - - - - - -

    //    //void CreateStepOffsets();

    //    #endregion Methods

    //}

    public interface INoiseGenerator : INoiseAttributes
    {

        #region - - - - - - Properties - - - - - -

        float[] NoiseData { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void InitNoiseGenerator(INoiseOffsetGenerator noiseOffsetGenerator);

        float[] SampleNoiseDataAtLocation(int mapSize, Vector3 position);

        #endregion Methods

    }

    public interface INoiseAttributes
    {

        #region - - - - Properties - - - -

        float NoiseScale { get; set; }
        float Persistence { get; set; }
        float Lacunarity { get; set; }
        int NoiseOctaveCount { get; set; }
        Vector2[] StepOffsets { get; set; }

        #endregion Properties

    }

    /// <summary>
    /// A class to generate noise for terrain processing.
    /// </summary>
    public class NoiseGenerator : GameHandler, INoiseGenerator
    {

        #region - - - - Fields - - - -

        private INoiseProcessor noiseProcessor;
        private INoiseInfoController infoController;
        private INoiseOffsetGenerator offsetGenerator;
        //private Vector2[] octavePositionOffsets;
        private float[] noiseData;

        #endregion Fields

        #region - - - - Properties - - - -

        public float[] NoiseData => noiseData;
        public float NoiseScale { get; set; }
        public float Persistence { get; set; }
        public float Lacunarity { get; set; }
        public int NoiseOctaveCount { get; set; }
        public Vector2[] StepOffsets { get; set; }

        #endregion Properties

        #region - - - - - - MonoBehaviour - - - - - -

        public void Awake()
        {
            noiseProcessor = this.GetComponent<INoiseProcessor>();
            //seedGenerator = this.GetComponent<ISeedGenerator>();

            infoController = FindObjectOfType<NoiseInfoController>(); // Can be Better
            if (infoController != null)
                infoController.GetNoiseAttributes(this);
        }

        #endregion MonoBehaviour

        #region - - - - - - Methods - - - - - -

        public void InitNoiseGenerator(INoiseOffsetGenerator noiseOffsetGenerator)
            => this.offsetGenerator = noiseOffsetGenerator;


        public float[] SampleNoiseDataAtLocation(int mapSize, Vector3 samplePosition)
        {
            noiseData = new float[25 * 25];
            noiseProcessor.ProcessNoiseData(noiseData, 25, samplePosition);

            return noiseData;
        }

        /// <summary>
        /// Creates position offsets against sample position for noise data complexity.
        /// </summary>
        //public void CreateStepOffsets()
        //{
        //    System.Random psuedoRandNumbGenerator = new System.Random(seedGenerator.Seed);
        //    octavePositionOffsets = new Vector2[NoiseOctaveCount];
        //    for (int i = 0; i < NoiseOctaveCount; i++)
        //    {
        //        octavePositionOffsets[i].x = psuedoRandNumbGenerator.Next(-100000, 100000);
        //        octavePositionOffsets[i].y = psuedoRandNumbGenerator.Next(-100000, 100000);
        //        print(octavePositionOffsets[i].x + ", " + octavePositionOffsets[i].y);
        //    }
        //}

        #endregion Methods

    }

}
