using UnityEngine;

namespace MiniProceduralGeneration.Noise.Processor
{

    public interface INoiseProcessor
    {

        #region - - - - - - Methods - - - - - -

        void ProcessNoiseData(float[] noiseDataArray, int mapSize, Vector3 samplePosition);

        #endregion Methods

    }

}
