using UnityEngine;

namespace MiniProceduralGeneration.Generator.Seed
{

    public interface ISeedGenerator
    {
        bool HasCreatedSeed { get; }
        int Seed { get; set; }
        void GenerateSeed();
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