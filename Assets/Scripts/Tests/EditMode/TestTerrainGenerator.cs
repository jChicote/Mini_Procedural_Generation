using MiniProceduralGeneration.Generator;
using NUnit.Framework;
using UnityEngine;

namespace MiniProceduralGeneration.Test.EditMode
{
    public class TestTerrainGenerator
    {
        [Test]
        public void CheckMapSizeIsDivisibleByEven()
        {
            TerrainManager generatorInstance = GameObject.FindWithTag("TerrainGenerator").GetComponent<TerrainManager>();
            bool checkCondition = (generatorInstance.RenderChunkSize % 2) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void CheckMapSizeIsDivisibleByThree()
        {
            TerrainManager generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainManager>();
            bool checkCondition = (generatorInstance.RenderChunkSize % 3) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void ScaledSizeIsEqualToOriginalMapSize()
        {
            /*TerrainManager generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainManager>();
            generatorInstance.CalculateChunkDimensions();
            float scaledSize = generatorInstance.VertexPerSide * generatorInstance.LODIncrementStep;

            Assert.AreEqual(generatorInstance.ChunkWidth, scaledSize);*/
        }
    }
}
