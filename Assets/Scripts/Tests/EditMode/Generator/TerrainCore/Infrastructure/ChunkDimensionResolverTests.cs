using MiniProceduralGeneration.TerrainCore;
using MiniProceduralGeneration.TerrainCore.Infrastructure;
using NUnit.Framework;
using Moq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace MiniProceduralGeneration.Tests.EditMode.TerrainCore.Infrastructure
{

    public class ChunkDimensionResolverTests
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


        }

        #region - - - - - - Methods - - - - - -

        [UnityTest]
        public IEnumerator CalculateDistanceToTileCenter_TestVectorPositionWithTargetObjectAtCenter_ValidDistance()
        {
            // Arrange
            var expected = 5;
            var testPosition = new Vector3(5, 0, 0);

            var mockTargetTrackker = new Mock<ITargetObjectTracker>();
            mockTargetTrackker
                .Setup(mock => mock.TargetPositionInWorldSpace)
                .Returns(new Vector3(0, 0, 0));

            GameObject gameObject = new GameObject();
            ChunkDimensionsResolver resolver = gameObject.AddComponent<ChunkDimensionsResolver>();

            // Act
            var actual = resolver.CalculateDistanceToTileCenter(testPosition);

            // Assert
            Assert.AreEqual(expected, actual);
            yield return null;
        }

        #endregion Methods

    }

}
