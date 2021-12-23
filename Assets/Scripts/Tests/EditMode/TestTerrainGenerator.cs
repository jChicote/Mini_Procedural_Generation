using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MiniProceduralGeneration.Generator;
using MiniProceduralGeneration.Generator.Processor;

namespace MiniProceduralGeneration.Test.EditMode
{
    public class TestTerrainGenerator
    {
        [Test]
        public void CheckMapSizeIsDivisibleByEven()
        {
            TerrainGenerator generatorInstance = GameObject.FindWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            bool checkCondition = (generatorInstance.MapSize % 2) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void CheckMapSizeIsDivisibleByThree()
        {
            TerrainGenerator generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            bool checkCondition = (generatorInstance.MapSize % 3) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void ScaledSizeIsEqualToOriginalMapSize()
        {
            TerrainGenerator generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            generatorInstance.CalculateChunkDimensions();
            float scaledSize = generatorInstance.VertexPerSide * generatorInstance.LODIncrementStep;

            Assert.AreEqual(generatorInstance.MapSize, scaledSize);
        }

        [Test]
        public void MinimumLODIsEqualToRecursiveLevelOfDetail()
        {
            TerrainGenerator generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            int lod = 0;
            int minimumLevelOfDetail = generatorInstance.FindMininmumAllowableLevelOfDetail(lod);

            Debug.Log("Minimum LOD >> " + minimumLevelOfDetail);

            Assert.AreEqual(12 , minimumLevelOfDetail);
        }
    }
}
