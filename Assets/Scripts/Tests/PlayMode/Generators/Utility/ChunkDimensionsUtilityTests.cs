using MiniProceduralGeneration.Generator;
using MiniProceduralGeneration.Generator.Utility;
using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayMode.Generators.Utility
{

    public class ChunkDimensionsUtilityTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<ITerrainAttributes> mockTerrainAttributes;

        static Vector2[] lodValues = new Vector2[] { new Vector2(24, 3) };

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ChunkDimensionsUtilityTests()
        {
            mockTerrainAttributes = new Mock<ITerrainAttributes>();
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        [UnityTest]
        public IEnumerator FindMininmumAllowableLevelOfDetail_EvenValues_SmallestEvenDivisibleValue(
            [ValueSource("lodValues")] Vector2 lodTestValues)
        {
            // Arrange
            var expected = lodTestValues.y;
            var dimensionsUtility = new ChunkDimensionsUtility(mockTerrainAttributes.Object);

            mockTerrainAttributes.Setup(mock => mock.RenderChunkSize).Returns(24);

            // Act
            var actual = dimensionsUtility.FindMininmumAllowableLevelOfDetail(0, (int)lodTestValues.x);

            // Assert
            Assert.AreEqual((int)expected, actual);
            yield return null;
        }

        #endregion

    }

}
