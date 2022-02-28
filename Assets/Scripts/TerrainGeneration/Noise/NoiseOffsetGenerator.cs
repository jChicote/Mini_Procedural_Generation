using MiniProceduralGeneration.Seed;
using UnityEngine;

namespace MiniProceduralGeneration.Noise
{

    public interface INoiseOffsetGenerator
    {

        #region - - - - - - Fields - - - - - -

        Vector2[] OctaveOffsets { get; }

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        Vector2[] CreateStepOffsets(int octaveCount);

        #endregion Methods

    }

    public class NoiseOffsetGenerator : MonoBehaviour, INoiseOffsetGenerator
    {

        #region - - - - - - Fields - - - - - -

        private ISeedGenerator m_SeedGenerator;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public Vector2[] OctaveOffsets { get; private set; }

        #endregion Properties

        #region - - - - - - MonoBehaviour - - - - - -

        private void Awake()
            => this.m_SeedGenerator = this.GetComponent<ISeedGenerator>();

        #endregion MonoBehaviour

        #region - - - - - - Methods - - - - - -

        public Vector2[] CreateStepOffsets(int octaveCount)
        {
            System.Random psuedoRandNumbGenerator = new System.Random(m_SeedGenerator.Seed);
            OctaveOffsets = new Vector2[octaveCount];
            for (int i = 0; i < octaveCount; i++)
            {
                OctaveOffsets[i].x = psuedoRandNumbGenerator.Next(-100000, 100000);
                OctaveOffsets[i].y = psuedoRandNumbGenerator.Next(-100000, 100000);
            }

            return OctaveOffsets;
        }

        #endregion Methods

    }

}
