using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MiniProceduralGeneration.Generator;

namespace MiniProceduralGeneration.Test.Playmode
{
    public class TestActiveTerrainGenerator
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestRun()
        {
            Assert.AreEqual(true, true);
        }

        [Test]
        public void CheckMapSizeIsDivisibleByEven()
        {
            TerrainManager generatorInstance = GameObject.FindWithTag("TerrainGenerator").GetComponent<TerrainManager>();
            bool checkCondition = (generatorInstance.ChunkWidth % 2) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void CheckMapSizeIsDivisibleByThree()
        {
            TerrainManager generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainManager>();

        }
    }
}
