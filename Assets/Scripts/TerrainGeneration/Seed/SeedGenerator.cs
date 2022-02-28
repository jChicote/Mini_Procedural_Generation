using UnityEngine;

namespace MiniProceduralGeneration.Seed
{

    public interface ISeedGenerator
    {

        #region - - - - - - Properties - - - - - -

        bool HasCreatedSeed { get; }
        int Seed { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - -

        void GenerateSeed();

        #endregion Methods

    }

    public class SeedGenerator : MonoBehaviour, ISeedGenerator
    {

        #region - - - - Properties - - - -

        public bool HasCreatedSeed => Seed != 0;
        public int Seed { get; set; }

        #endregion Properties

        #region - - - - Methods - - - -

        public void Awake()
        {
            Seed = 0;
        }

        public void GenerateSeed()
        {
            Seed = Random.Range(1, 1000000);
            print("Seed Created: " + Seed);
        }

        #endregion Methods

    }

}