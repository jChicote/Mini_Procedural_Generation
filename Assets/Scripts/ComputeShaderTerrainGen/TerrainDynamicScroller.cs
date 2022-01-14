using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniProceduralGeneration.Generator.MeshWork;


namespace MiniProceduralGeneration.Generator.DynamicFeature
{

    /// <summary>
    /// 
    /// !!!!!!!!!!!!!!!!! IMPORTANT TO CHANGE !!!!!!!!!!!!!!!!!!!!
    /// 
    /// Refactoring Chores needed for this class:
    /// - Changing Terrain Dynamic scroller to be the parent controller of both scrolling and chunk rendering
    /// - Collapse scrolling motion to single function controlled by integer direction
    /// - Refactor map chunk renddering with better name
    /// 
    /// </summary>
    /// 
    public class TerrainDynamicScroller : MonoBehaviour
    {
        // Fields
        private ITerrainChunkArray chunkArrayInterface;
        public Transform targetObject;
        public GameObject chunkPrefab;

        public int chunkDistance = 4;
        private int chunkDimension = 0;

        int rightmostEdgeCol = 0;
        int leftMostEdgeCol = 0;
        int topMostEdgeRow = 0;
        int bottomMostEdgeRow = 0;

        ITerrainChunk leftChunk;
        ITerrainChunk rightChunk;
        ITerrainChunk topChunk;
        ITerrainChunk bottomChunk;

        ITerrainCharacteristics characteristics;

        private void Awake()
        {
            chunkArrayInterface = this.GetComponent<ITerrainChunkArray>();
            characteristics = this.GetComponent<ITerrainCharacteristics>();
        }

        private void Update()
        {
            if (chunkArrayInterface.TerrainChunks.Length <= 1) return;

            ScrollMap();
        }

        /// <summary>
        /// Checks wether target object exceeds compared limits to trigger chunk shifting.
        /// </summary>
        private void ScrollMap()
        {
            float halfDistance = chunkDistance * characteristics.MapSize;
            if (targetObject.position.x < leftChunk.PositionWorldSpace.x + halfDistance) // Reposition Left
            {
                RepositionColToLeft();
            }
            else if (targetObject.position.x > rightChunk.PositionWorldSpace.x - halfDistance / 2)// - (chunkDistance / 2) * characteristics.MapSize)) // Reposition Right
            {
                RepositionColToRight();
            }
            else if (targetObject.position.z > topChunk.PositionWorldSpace.z - halfDistance / 2) // Reposition Up
            {
                RepositionRowToTop();
            }
            else if (targetObject.position.z < bottomChunk.PositionWorldSpace.z + halfDistance) // Reposition Down
            {
                RepositionRowToBottom();
            }
        }


        private void RepositionColToLeft()
        {
            ShiftAndEnumerateHorizontalCol(rightmostEdgeCol, -1);

            leftMostEdgeCol = rightmostEdgeCol;
            rightmostEdgeCol--;

            if (rightmostEdgeCol < 0)
            {
                rightmostEdgeCol = chunkDistance * 2;
            }

            DefineReferenceChunksInCardinalDirections();
        }

        private void RepositionColToRight()
        {
            ShiftAndEnumerateHorizontalCol(leftMostEdgeCol, 1);

            rightmostEdgeCol = leftMostEdgeCol;
            leftMostEdgeCol++;

            if (leftMostEdgeCol == chunkDimension)
            {
                leftMostEdgeCol = 0;
            }

            DefineReferenceChunksInCardinalDirections();
        }

        /// <summary>
        /// Iterates through chunks to shift chunks to a specified column in the map.
        /// </summary>
        private void ShiftAndEnumerateHorizontalCol(int targetCol, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < chunkDistance * 2 + 1; i++)
            {
                index = GetIndexFromRowAndCol(i, targetCol);
                newPosition = chunkArrayInterface.TerrainChunks[index].PositionWorldSpace;
                newPosition.x += characteristics.MapSize * chunkDimension * movementDirection;
                chunkArrayInterface.TerrainChunks[index].PositionWorldSpace = newPosition;
            }
        }

        private void RepositionRowToTop()
        {
            ShiftAndEnumerateVerticalRow(bottomMostEdgeRow, 1);

            topMostEdgeRow = bottomMostEdgeRow;
            bottomMostEdgeRow--;

            if (bottomMostEdgeRow < 0)
            {
                bottomMostEdgeRow = chunkDistance * 2;
            }

            DefineReferenceChunksInCardinalDirections();
        }

        private void RepositionRowToBottom()
        {
            ShiftAndEnumerateVerticalRow(topMostEdgeRow, -1);

            bottomMostEdgeRow = topMostEdgeRow;
            topMostEdgeRow++;

            if (topMostEdgeRow == chunkDimension)
            {
                topMostEdgeRow = 0;
            }

            DefineReferenceChunksInCardinalDirections();
        }

        /// <summary>
        /// Iterates through chuinks to shift chu7nks to a specified row in the map.
        /// </summary>
        private void ShiftAndEnumerateVerticalRow(int targetRow, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < chunkDimension; i++)
            {
                index = GetIndexFromRowAndCol(targetRow, i);
                newPosition = chunkArrayInterface.TerrainChunks[index].PositionWorldSpace;
                newPosition.z += characteristics.MapSize * chunkDimension * movementDirection;
                chunkArrayInterface.TerrainChunks[index].PositionWorldSpace = newPosition;
            }
        }

        private int GetIndexFromRowAndCol(int row, int col)
        {
            return row * (chunkDistance * 2 + 1) + col;
        }

        /// <summary>
        /// Creates a completed terrain map with a collection of seperate chunks
        /// </summary>
        public void CreateChunkMap()
        {
            ClearMap();
            DefineScrollingIndexCases();

            // chunk scroll backtracks
            for (int z = chunkDistance; z > -chunkDistance - 1; z--)
            {
                for (int x = -chunkDistance; x < chunkDistance + 1; x++)
                {
                    CreateChunk(new Vector3((x * characteristics.MapSize) + targetObject.position.x, 0, (z * characteristics.MapSize) + targetObject.position.z));
                }
            }

            DefineReferenceChunksInCardinalDirections();
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
            chunkDimension = chunkDistance * 2 + 1;
            rightmostEdgeCol = chunkDistance * 2;
            leftMostEdgeCol = 0;
            topMostEdgeRow = 0;
            bottomMostEdgeRow = chunkDimension - 1;
        }

        private void DefineReferenceChunksInCardinalDirections()
        {
            leftChunk = chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(0, leftMostEdgeCol)];
            rightChunk = chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(0, rightmostEdgeCol)];
            topChunk = chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(topMostEdgeRow, 0)];
            bottomChunk = chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(bottomMostEdgeRow, 0)];
        }

        private void DestroyChunk(ITerrainChunk chunk, int index)
        {
            chunkArrayInterface.TerrainChunks[index] = null;
            chunk.OnDestroyChunk();
        }

        private void OnDrawGizmos()
        {
            ITerrainCharacteristics characteristics = this.GetComponent<ITerrainCharacteristics>();

            if (leftChunk != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(leftChunk.PositionWorldSpace, 3);
                Gizmos.DrawSphere(new Vector3(rightChunk.PositionWorldSpace.x + 24, rightChunk.PositionWorldSpace.y, rightChunk.PositionWorldSpace.z), 3);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(new Vector3(leftChunk.PositionWorldSpace.x + (chunkDistance / 2) * characteristics.MapSize, 0, 0), Vector3.one * 3);
                Gizmos.DrawCube(new Vector3((rightChunk.PositionWorldSpace.x - (chunkDistance / 2) * characteristics.MapSize) + characteristics.MapSize, 0, 0), Vector3.one * 3);
            }

            if (leftChunk != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rightChunk.PositionWorldSpace, 3);
            }

            if (chunkArrayInterface.TerrainChunks.Length > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(chunkArrayInterface.TerrainChunks[GetIndexFromRowAndCol(topMostEdgeRow, 0)].PositionWorldSpace, 4);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(chunkArrayInterface.TerrainChunks[chunkArrayInterface.TerrainChunks.Length - 1].PositionWorldSpace, 4);
            }
        }
    }
}
