using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using UnityEngine;
using UnityEngine.TestTools;

namespace MiniProceduralGeneration.Tests.EditMode.Generator.Noise
{

    public class NoiseGeneratorTests
    {
        #region - - - - Fields - - - -

        public GameObject noiseGameObject;
        public readonly NoiseGenerator noiseGenerator;
        public readonly SeedGenerator seedGenerator;

        #endregion

        #region - - - - Constructors - - - -

        public NoiseGeneratorTests()
        {
            noiseGameObject = new GameObject();
            noiseGameObject.AddComponent<NoiseGenerator>();
            noiseGameObject.AddComponent<SeedGenerator>();
        }

        #endregion Constructors

        #region - - - - Tests - - - -

        [UnityTest]
        public void CreateStepOffsets_InvokeMethod_GeneratedOffsetValues()
        {
            // Arrange
            //noiseGameObject.AddComponent<NoiseGenerator>();
            //noiseGameObject.AddComponent<SeedGenerator>();

            //noiseGenerator.Awake();
            //seedGenerator.Awake();
            //seedGenerator.GenerateSeed();

            //INoiseAttributes attributes = noiseGameObject.GetComponent<INoiseAttributes>();
            //attributes.NoiseOctaveCount = 3;

            //// Act
            //noiseGenerator.CreateStepOffsets();

            //// Assert
            //Assert.AreNotSame(Vector2.zero, attributes.StepOffsets);
        }

        #endregion Tests
    }

}
