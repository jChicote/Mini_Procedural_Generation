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

            Assert.AreEqual(generatorInstance.MapSize + 1, scaledSize);
        }

        [Test]
        public void NoiseArrayIsEqualToDefaultMeshArraySize()
        {
            TerrainGenerator generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            Vector3[] vertices = new Vector3[generatorInstance.MapSize * generatorInstance.MapSize];
            float[] noiseData = new float[generatorInstance.MapSize * generatorInstance.MapSize];

            Assert.AreEqual(noiseData.Length, vertices.Length);
        }

        [Test]
        public void NoiseDimensionsCanScaleWithTerrainSize()
        {

        }
    }
}
