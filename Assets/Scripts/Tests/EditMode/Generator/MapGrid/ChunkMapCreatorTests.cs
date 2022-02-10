using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.MeshWork;
using NUnit.Framework;
using System;
using UnityEngine;

namespace MiniProceduralGeneration.Tests.EditMode.Generator.MapGrid
{

    public class ChunkMapCreatorTests
    {
        #region - - - - Fields - - - -

        private ChunkMapCreator mapCreator;
        private readonly GameObject chunkPrefab;

        #endregion Fields

        #region - - - - Constructor - - - -

        public ChunkMapCreatorTests()
        {
            mapCreator = new ChunkMapCreator();

            chunkPrefab = new GameObject();
            chunkPrefab.AddComponent<ChunkBody>();
        }

        #endregion Constructor

        [Test]
        public void CreateChunk_ValidPositionVector_CreatedChunkInstance()
        {

        }

        [Test]
        public void ClearMap_FilledChunkArray_SuccessfulInvocation()
        {
            // Arrage
            /*ITerrainChunk[] chunkArray = new ITerrainChunk[65];
            for(int i = 0; i < chunkArray.Length; i++)
                chunkArray[i] = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity).GetComponent<ITerrainChunk>();

            // Act
            mapCreator.ClearMap(chunkArray);

            // Assert
            Assert.AreEqual(0, chunkArray.Length);*/
        }

        private object Instantiate(object chunkPrefab, object newPosition, object identity)
        {
            throw new NotImplementedException();
        }
    }

}