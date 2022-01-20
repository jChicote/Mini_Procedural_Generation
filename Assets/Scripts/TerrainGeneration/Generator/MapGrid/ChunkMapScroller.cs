using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.Utility;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.Scroller
{

    public class ChunkMapScroller : GameHandler
    {

        #region -------- Fields --------

        public Transform targetObject;
        public GameObject chunkPrefab;
        public MapBorder mapBorder;
        private int mapGridEdgeSize = 0;

        private ITerrainChunks terrainChunks;
        private ITerrainAttributes terrainAttributes;
        private IMapGridCreator mapGridCreator;

        #endregion

        #region ------ Properties ------

        public MapBorder MapBorder => mapBorder;

        #endregion Properties

        #region ------ Methods ------

        public override object Handle(object request)
        {
            terrainChunks = this.GetComponent<ITerrainChunks>();
            terrainAttributes = this.GetComponent<ITerrainAttributes>();
            mapGridCreator = this.GetComponent<IMapGridCreator>();
            mapBorder = new MapBorder();

            DefineMapBorders();

            return base.Handle(request);
        }

        private void Update()
        {
            if (terrainChunks.ChunkArray.Length <= 1) return;

            ScrollMap();
        }

        public void DefineMapBorders()
        {
            mapGridEdgeSize = mapGridCreator.ChunkDistance * 2 + 1;

            mapBorder.FindMapBoundaryIndexes(mapGridCreator.ChunkDistance, mapGridEdgeSize);
            mapBorder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        /// <summary>
        /// Checks wether target object exceeds compared limits to trigger chunk shifting.
        /// </summary>
        private void ScrollMap()
        {
            float halfDistance = mapGridCreator.ChunkDistance * terrainAttributes.ChunkWidth;
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
                mapBorder.RightmostEdgeCol = mapGridCreator.ChunkDistance * 2;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        private void RepositionColToRight()
        {
            ShiftAndEnumerateHorizontalCol(mapBorder.LeftMostEdgeCol, 1);

            mapBorder.RightmostEdgeCol = mapBorder.LeftMostEdgeCol;
            mapBorder.LeftMostEdgeCol++;

            if (mapBorder.LeftMostEdgeCol == mapGridEdgeSize)
            {
                mapBorder.LeftMostEdgeCol = 0;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        /// <summary>
        /// Iterates through chunks to shift chunks to a specified column in the map.
        /// </summary>
        private void ShiftAndEnumerateHorizontalCol(int targetCol, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < mapGridCreator.ChunkDistance * 2 + 1; i++)
            {
                index = MapArrayUtility.GetIndexFromRowAndCol(mapGridEdgeSize, i, targetCol);
                newPosition = terrainChunks.ChunkArray[index].PositionWorldSpace;
                newPosition.x += mapGridEdgeSize * terrainAttributes.ChunkWidth * movementDirection;
                terrainChunks.ChunkArray[index].PositionWorldSpace = newPosition;
            }
        }

        private void RepositionRowToTop()
        {
            ShiftAndEnumerateVerticalRow(mapBorder.BottomMostEdgeRow, 1);

            mapBorder.TopMostEdgeRow = mapBorder.BottomMostEdgeRow;
            mapBorder.BottomMostEdgeRow--;

            if (mapBorder.BottomMostEdgeRow < 0)
            {
                mapBorder.BottomMostEdgeRow = mapGridCreator.ChunkDistance * 2;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        private void RepositionRowToBottom()
        {
            ShiftAndEnumerateVerticalRow(mapBorder.TopMostEdgeRow, -1);

            mapBorder.BottomMostEdgeRow = mapBorder.TopMostEdgeRow;
            mapBorder.TopMostEdgeRow++;

            if (mapBorder.TopMostEdgeRow == mapGridEdgeSize)
            {
                mapBorder.TopMostEdgeRow = 0;
            }

            mapBorder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        /// <summary>
        /// Iterates through chuinks to shift chu7nks to a specified row in the map.
        /// </summary>
        private void ShiftAndEnumerateVerticalRow(int targetRow, int movementDirection)
        {
            int index;
            Vector3 newPosition;

            for (int i = 0; i < mapGridEdgeSize; i++)
            {
                index = MapArrayUtility.GetIndexFromRowAndCol(mapGridEdgeSize, targetRow, i);
                newPosition = terrainChunks.ChunkArray[index].PositionWorldSpace;
                newPosition.z += terrainAttributes.ChunkWidth * mapGridEdgeSize * movementDirection;
                terrainChunks.ChunkArray[index].PositionWorldSpace = newPosition;
            }
        }

        #endregion Methods
    }
}
