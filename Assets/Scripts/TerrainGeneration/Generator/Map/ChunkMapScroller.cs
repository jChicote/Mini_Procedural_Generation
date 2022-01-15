using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.Utility;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.Scroller
{

    public class ChunkMapScroller : MonoBehaviour
    {

        #region -------- Fields --------

        public Transform targetObject;
        public GameObject chunkPrefab;
        public ChunkMap chunkMap;
        public MapBorder mapBorder;

        #endregion

        #region ------ Properties ------

        public MapBorder MapBorder => mapBorder;

        #endregion Properties

        #region ------ Methods ------

        private void Start()
        {
            chunkMap = this.GetComponent<IMapCreator>().ChunkMap;

            mapBorder = new MapBorder();
        }

        private void Update()
        {
            if (chunkMap.ChunkArrayInterface.TerrainChunks.Length <= 1) return;

            ScrollMap();
        }

        public void DefineMapBorders()
        {
            mapBorder.FindMapBoundaryIndexes(chunkMap.ChunkDistance, chunkMap.MapEdgeSize);
            mapBorder.DefineReferenceChunksInCardinalDirections(chunkMap.ChunkArrayInterface, chunkMap.MapEdgeSize);
        }

        /// <summary>
        /// Checks wether target object exceeds compared limits to trigger chunk shifting.
        /// </summary>
        private void ScrollMap()
        {
            float halfDistance = chunkMap.ChunkDistance * chunkMap.Characteristics.MapSize;
            if (targetObject.position.x < mapBorder.LeftChunk.PositionWorldSpace.x + halfDistance) // Reposition Left
            {
                RepositionColToLeft();
            }
            else if (targetObject.position.x > mapBorder.RightChunk.PositionWorldSpace.x - halfDistance / 2)// - (chunkDistance / 2) * characteristics.MapSize)) // Reposition Right
            {
                RepositionColToRight();
            }
            else if (targetObject.position.z > mapBorder.TopChunk.PositionWorldSpace.z - halfDistance / 2) // Reposition Up
            {
                RepositionRowToTop();
            }
            else if (targetObject.position.z < mapBorder.BottomChunk.PositionWorldSpace.z + halfDistance) // Reposition Down
            {
                RepositionRowToBottom();
            }
        }


        private void RepositionColToLeft()
        {
            ShiftAndEnumerateHorizontalCol(mapBorder.RightmostEdgeCol, -1);

            mapBorder.LeftMostEdgeCol = mapBorder.RightmostEdgeCol;
            mapBorder.RightmostEdgeCol--;

            if (mapBorder.RightmostEdgeCol < 0)
            {
                mapBorder.RightmostEdgeCol = chunkMap.ChunkDistance * 2;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(chunkMap.ChunkArrayInterface, chunkMap.MapEdgeSize);
        }

        private void RepositionColToRight()
        {
            ShiftAndEnumerateHorizontalCol(mapBorder.LeftMostEdgeCol, 1);

            mapBorder.RightmostEdgeCol = mapBorder.LeftMostEdgeCol;
            mapBorder.LeftMostEdgeCol++;

            if (mapBorder.LeftMostEdgeCol == chunkMap.MapEdgeSize)
            {
                mapBorder.LeftMostEdgeCol = 0;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(chunkMap.ChunkArrayInterface, chunkMap.MapEdgeSize);
        }

        /// <summary>
        /// Iterates through chunks to shift chunks to a specified column in the map.
        /// </summary>
        private void ShiftAndEnumerateHorizontalCol(int targetCol, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < chunkMap.ChunkDistance * 2 + 1; i++)
            {
                index = MapArrayUtility.GetIndexFromRowAndCol(chunkMap.MapEdgeSize, i, targetCol);
                newPosition = chunkMap.ChunkArrayInterface.TerrainChunks[index].PositionWorldSpace;
                newPosition.x += chunkMap.MapEdgeSize * chunkMap.Characteristics.MapSize * movementDirection;
                chunkMap.ChunkArrayInterface.TerrainChunks[index].PositionWorldSpace = newPosition;
            }
        }

        private void RepositionRowToTop()
        {
            ShiftAndEnumerateVerticalRow(mapBorder.BottomMostEdgeRow, 1);

            mapBorder.TopMostEdgeRow = mapBorder.BottomMostEdgeRow;
            mapBorder.BottomMostEdgeRow--;

            if (mapBorder.BottomMostEdgeRow < 0)
            {
                mapBorder.BottomMostEdgeRow = chunkMap.ChunkDistance * 2;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(chunkMap.ChunkArrayInterface, chunkMap.MapEdgeSize);
        }

        private void RepositionRowToBottom()
        {
            ShiftAndEnumerateVerticalRow(mapBorder.TopMostEdgeRow, -1);

            mapBorder.BottomMostEdgeRow = mapBorder.TopMostEdgeRow;
            mapBorder.TopMostEdgeRow++;

            if (mapBorder.TopMostEdgeRow == chunkMap.MapEdgeSize)
            {
                mapBorder.TopMostEdgeRow = 0;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(chunkMap.ChunkArrayInterface, chunkMap.MapEdgeSize);
        }

        /// <summary>
        /// Iterates through chuinks to shift chu7nks to a specified row in the map.
        /// </summary>
        private void ShiftAndEnumerateVerticalRow(int targetRow, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < chunkMap.MapEdgeSize; i++)
            {
                index = MapArrayUtility.GetIndexFromRowAndCol(chunkMap.MapEdgeSize, targetRow, i);
                newPosition = chunkMap.ChunkArrayInterface.TerrainChunks[index].PositionWorldSpace;
                newPosition.z += chunkMap.Characteristics.MapSize * chunkMap.MapEdgeSize * movementDirection;
                chunkMap.ChunkArrayInterface.TerrainChunks[index].PositionWorldSpace = newPosition;
            }
        }

        #endregion Methods
    }
}
