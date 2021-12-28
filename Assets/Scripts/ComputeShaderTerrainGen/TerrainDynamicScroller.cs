using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.DynamicFeature
{
    public class TerrainDynamicScroller : MonoBehaviour
    {
        // Fields
        private ITerrainChunkArray chunkArrayInterface;
        public Transform targetObject;
        public GameObject chunkPrefab;

        private GameObject[] testChunksArray;

        public int chunkDistance = 4;
        private int chunkDimension = 0;

        private void Awake()
        {
            chunkArrayInterface = this.GetComponent<ITerrainChunkArray>();

            testChunksArray = new GameObject[2];
        }


        // ---------------------------------------
        //              Target Tracker
        // ---------------------------------------


        // ---------------------------------------
        //              Chunk Renderer
        // ---------------------------------------

        public void CreateChunkMap()
        {
            ClearMap();

            ITerrainCharacteristics characteristics = this.GetComponent<ITerrainCharacteristics>();
            
            Vector3 testPosition = new Vector3(0, 0, 0);
            Vector3 offsetStartPosition = new Vector3(0, 0, 0);
            offsetStartPosition.x = testPosition.x + chunkDistance * characteristics.MapSize;
            offsetStartPosition.z = testPosition.z + chunkDistance * characteristics.MapSize;

            // chunk scroll backtracks
            for (int x = chunkDistance; x > -chunkDistance - 1; x--)
            {
                for (int z = chunkDistance; z > -chunkDistance - 1; z--) {
                    CreateChunk(new Vector3(x* characteristics.MapSize, 0,z * characteristics.MapSize));
                }
            }
        }

        public void CreateChunk(Vector3 newPosition)
        {
            ITerrainChunk[] tempArray = new ITerrainChunk[chunkArrayInterface.TerrainChunks.Length + 1];

            for (int i = 0; i < chunkArrayInterface.TerrainChunks.Length; i++)
            {
                tempArray[i] = chunkArrayInterface.TerrainChunks[i];
            }

            tempArray[tempArray.Length - 1] = Instantiate(chunkPrefab, newPosition, Quaternion.identity).GetComponent<ITerrainChunk>();
            chunkArrayInterface.TerrainChunks = tempArray;
        }

        private void ClearMap()
        {
            if (chunkArrayInterface.TerrainChunks.Length == 0) return;

            ITerrainChunk chunk;
            int index = chunkArrayInterface.TerrainChunks.Length - 1;

            while (index >= 0)
            {
                chunk = chunkArrayInterface.TerrainChunks[index];
                DestroyChunk(chunk, index);
                index--;
            }

            chunkDimension = (chunkDistance * chunkDistance + 1);
            chunkArrayInterface.TerrainChunks = new ITerrainChunk[0];
        }

        private void DestroyChunk(ITerrainChunk chunk, int index)
        {
            chunkArrayInterface.TerrainChunks[index] = null;
            chunk.OnDestroyChunk();
        }
    }
}
