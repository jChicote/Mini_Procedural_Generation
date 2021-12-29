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

        int rightmostEdgeCol = 0;
        int leftMostEdgeCol = 0;
        int topMostEdgeCol = 0;
        int bottomMostCol = 0;
        int centerCol = 0;

        ITerrainChunk leftChunk;

        private void Awake()
        {
            chunkArrayInterface = this.GetComponent<ITerrainChunkArray>();

            testChunksArray = new GameObject[2];
        }

        private void Update()
        {
            if (chunkArrayInterface.TerrainChunks.Length <= 1) return;

            ScrollMap();
        }


        // ---------------------------------------
        //              Target Tracker
        // ---------------------------------------

        /// <summary>
        /// 
        /// Objective:
        /// - Vertically scroll map depending on vertical chunks 
        /// - Logs current row and column to determine which is the edge col and row
        /// - individually process positioned chunks to new set position
        /// - Does not manuever array items instead references current edge row indexes
        /// 
        /// </summary>
        private void ScrollMap()
        {
            chunkDimension = chunkDistance * 2 + 1;

            leftChunk = chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(chunkDistance, leftMostEdgeCol)];
            //bool test = true;


            // Reposition Left
            if (targetObject.position.x < leftChunk.PositionWorldSpace.x + chunkDistance / 2)
            {
                print("Player: " + targetObject.position.x + ", Compared: " + leftChunk.PositionWorldSpace.x + ", Index: " + leftMostEdgeCol);
                RepositionColToLeft();
                //print(leftChunk.PositionWorldSpace.x + ((chunkDistance / 2) * 24)); // FOUND THIS TO BE THE CULPRINT, SHIFTS ALL TILES WHEN ALL CHUNKS EVENTUALLY SHIFTS ALL WHEN IT MEETS CONDITION WHEN EACH TILE IS SHIFTED
                //test = false;
            }

            // MAJOR ISSUES CAUSED BY ORIENTATION OF X AND Y REFERENCING MESHES INCORRECTLY.
            // THE TOP RIGHT IS THE MOST CORNER POINT FROM 0,0.

            // Repoisition Right

        }

        private int GetIndexFromRowAndCol(int row, int col)
        {
            return row * (chunkDistance * 2 + 1) + col;
        }

        private void RepositionColToLeft()
        {
            ITerrainCharacteristics characteristics = this.GetComponent<ITerrainCharacteristics>();

            for (int i = 0; i < chunkDistance * 2 + 1; i++) {
                int index = GetIndexFromRowAndCol(i, rightmostEdgeCol);
                Vector3 newPosition = chunkArrayInterface.TerrainChunks[index].PositionWorldSpace;
                newPosition.x -= characteristics.MapSize * chunkDimension;
                chunkArrayInterface.TerrainChunks[index].PositionWorldSpace = newPosition;
                print(chunkDistance * 2 + 1);
            }

            leftMostEdgeCol = rightmostEdgeCol;
            rightmostEdgeCol--;

            if (rightmostEdgeCol < 0)
            {
                rightmostEdgeCol = chunkDistance * 2;
            }
        }

        private int RepositionColToRight()
        {
            return 0;
        }

        // ---------------------------------------
        //              Chunk Renderer
        // ---------------------------------------

        public void CreateChunkMap()
        {
            ClearMap();
            DefineScrollingIndexCases();

            ITerrainCharacteristics characteristics = this.GetComponent<ITerrainCharacteristics>();
            
            Vector3 testPosition = targetObject.position;
            Vector3 offsetStartPosition = new Vector3(0, 0, 0);
            offsetStartPosition.x = testPosition.x + chunkDistance * characteristics.MapSize;
            offsetStartPosition.z = testPosition.z + chunkDistance * characteristics.MapSize;

            // chunk scroll backtracks
            for (int z = chunkDistance; z > -chunkDistance - 1; z--)
            {
                for (int x = -chunkDistance; x < chunkDistance + 1; x++) {
                    CreateChunk(new Vector3((x * characteristics.MapSize) + targetObject.position.x, 0, (z * characteristics.MapSize) + targetObject.position.z));
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

        private void DefineScrollingIndexCases()
        {
            rightmostEdgeCol = chunkDistance * 2;
            leftMostEdgeCol = 0;
            centerCol = chunkDistance;
        }

        private void DestroyChunk(ITerrainChunk chunk, int index)
        {
            chunkArrayInterface.TerrainChunks[index] = null;
            chunk.OnDestroyChunk();
        }

        private void OnDrawGizmos()
        {
            if (leftChunk != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(leftChunk.PositionWorldSpace, 3);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(new Vector3(leftChunk.PositionWorldSpace.x + (chunkDistance/2) * 24, 0, 0), Vector3.one * 3);
            }
        }
    }
}
