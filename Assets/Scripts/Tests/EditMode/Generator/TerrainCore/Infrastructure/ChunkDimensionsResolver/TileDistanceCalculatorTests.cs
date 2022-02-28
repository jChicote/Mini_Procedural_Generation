using MiniProceduralGeneration.MapGrid;
using MiniProceduralGeneration.TerrainCore;
using MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver;
using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MiniProceduralGeneration.Tests.EditMode.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class TileDistanceCalculatorTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IMapGridAttributes> mapGridAttributes = new Mock<IMapGridAttributes>();
        private readonly Mock<ITargetObjectTracker> targetTracker = new Mock<ITargetObjectTracker>();
        private readonly Mock<ITerrainAttributes> terrainAttributes = new Mock<ITerrainAttributes>();

        private readonly TileDistanceCalculator tileDistanceCalculator;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public TileDistanceCalculatorTests()
        {
            tileDistanceCalculator = new TileDistanceCalculator
            (
                mapGridAttributes.Object,
                targetTracker.Object,
                terrainAttributes.Object
            );

            this.mapGridAttributes
                .Setup(mock => mock.ChunkDistance)
                .Returns(4);

            this.targetTracker
                .Setup(mock => mock.TargetPositionInWorldSpace)
                .Returns(new Vector3(0, 0, 0));

            this.terrainAttributes
                .Setup(mock => mock.ActualChunkSize)
                .Returns(25);

            this.terrainAttributes
                .Setup(mock => mock.RenderChunkSize)
                .Returns(24);
        }

        #endregion Constructors

        #region - - - - - - CalculateDistanceToTileCenter Tests - - - - - -

        [UnityTest]
        public IEnumerator CalculateDistanceToTileCenter_ValidChunkPosition_ReturnsDistanceValue()
        {
            // Arrange
            var _expected = 37.9473305f;

            // Act
            var _actual = tileDistanceCalculator.CalculateDistanceToTileCenter(new Vector3(24, 0, 0));

            // Assert
            Assert.AreEqual(_expected, _actual);
            yield return null;
        }

        #endregion CalculateDistanceToTileCenter Tests

        #region - - - - - - CalculateLevelOfDetailDFromTileDistance Tests - - - - - -

        /// <summary>
        /// Test values for LOD distance inputs (negative values account for offset from the non-centered
        /// starting position of the tile chunk.
        /// </summary>
        private static TestIntValues<Vector3>[] lodDistanceTestValues = new TestIntValues<Vector3>[]
        {
            new TestIntValues<Vector3>() { expected = 1, inputValue = new Vector3(24, 0, 0) },
            new TestIntValues<Vector3>() { expected = 0, inputValue = new Vector3(-24, 0, 0) },
            new TestIntValues<Vector3>() { expected = 3, inputValue = new Vector3(72, 0, 0) },
            new TestIntValues<Vector3>() { expected = 2, inputValue = new Vector3(-72, 0, 0) },
            new TestIntValues<Vector3>() { expected = 5, inputValue = new Vector3(120, 0, 0) },
            new TestIntValues<Vector3>() { expected = 4, inputValue = new Vector3(-120, 0, 0) }
        };

        [UnityTest]
        public IEnumerator CalculateLevelOfDetailDFromTileDistance_ValidDistance_ReturnsValidLevelOfDetail([ValueSource("lodDistanceTestValues")] TestIntValues<Vector3> value)
        {
            // Arrange
            var _expected = value.expected;

            // Act
            var _actual = tileDistanceCalculator.CalculateLevelOfDetailDFromTileDistance(value.inputValue);

            // Assert
            Assert.AreEqual(_expected, _actual);
            yield return null;
        }

        #endregion CalculateLevelOfDetailDFromTileDistance Tests

    }

    public struct TestIntValues<T>
    {
        public int expected;
        public T inputValue;
    }
}
