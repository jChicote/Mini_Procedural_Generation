using MiniProceduralGeneration.TerrainCore;
using MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver;
using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace MiniProceduralGeneration.Tests.EditMode.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class MinimumLevelOfDetailIncrementFinderTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<ITerrainAttributes> terrainAttributes;

        private readonly MinimumLevelOfDetailIncrementFinder minimumIncrementFinder;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public MinimumLevelOfDetailIncrementFinderTests()
        {
            terrainAttributes = new Mock<ITerrainAttributes>();
            minimumIncrementFinder = new MinimumLevelOfDetailIncrementFinder(terrainAttributes.Object);

            this.terrainAttributes
                .Setup(mock => mock.RenderChunkSize)
                .Returns(24);
        }

        #endregion Constructors

        #region - - - - - - CalculateLevelOfDetailIncrement Tests - - - - - -

        [UnityTest]
        public IEnumerator CalculateLevelOfDetailIncrement_FullLevelOfDetailResolution_ReturnsFullResolutionIncrementStep()
        {
            // Arrange
            var _expected = 1;

            // Act
            var _actual = this.minimumIncrementFinder.CalculateLevelOfDetailIncrement(0);

            // Assert
            Assert.AreEqual(_expected, _actual);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CalculateLevelOfDetailIncrement_LevelOfDetailBeyondAllowableMinimum_ReturnsMinimumValue()
        {
            // Arrange
            var _expected = 6;

            // Act
            var _actual = this.minimumIncrementFinder.CalculateLevelOfDetailIncrement(6);

            // Assert
            Assert.AreEqual(_expected, _actual);
            yield return null;
        }

        #endregion CalculateLevelOfDetailIncrement Tests

        #region - - - - - - CalculateLevelOfDetailIncrement Tests - - - - - -

        [UnityTest]
        public IEnumerator FindMinimumAllowableLevelOfDetail_FullDetailResolution_ReturnsFullLevelOfDetail()
        {
            // Arrange
            var _expected = 3;

            // Act
            var _actual = this.minimumIncrementFinder.FindMininmumAllowableLevelOfDetail(0, 24);

            // Assert
            Assert.AreEqual(_expected, _actual);
            yield return null;
        }

        #endregion

    }

}
