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
            TerrainGenerator generatorInstance = GameObject.FindWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            bool checkCondition = (generatorInstance.MapSize % 2) == 0;

            Assert.AreEqual(true, checkCondition);
        }

        [Test]
        public void CheckMapSizeIsDivisibleByThree()
        {
            TerrainGenerator generatorInstance = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();

        }
    }
}
