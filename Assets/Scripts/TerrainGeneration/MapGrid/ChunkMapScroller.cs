using MiniProceduralGeneration.Handler;
using MiniProceduralGeneration.TerrainCore;
using MiniProceduralGeneration.Utility;
using UnityEngine;

namespace MiniProceduralGeneration.MapGrid
{

    public interface ICalculateMapBorder
    {
        void DefineMapBorders();
    }

    public class ChunkMapScroller : GameHandler, ICalculateMapBorder
    {

        #region - - - - - - - - Fields - - - - - - - -

        public Transform targetObject;
        public GameObject chunkPrefab;
        private int mapGridEdgeSize = 0;

        private ITerrainChunkCollection terrainChunks;
        private ITerrainRunnerAction terrainRunner;
        private ITerrainAttributes terrainAttributes;
        private IMapGridCreator mapGridCreator;
        private IMapGridBorderFinder mapBorderFinder;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public override object AwakeHandle(object request)
        {
            terrainChunks = this.GetComponent<ITerrainChunkCollection>();
            terrainAttributes = this.GetComponent<ITerrainAttributes>();
            terrainRunner = this.GetComponent<ITerrainRunnerAction>();
            mapGridCreator = this.GetComponent<IMapGridCreator>();
            mapBorderFinder = new MapGridBorderFinder();

            DefineMapBorders();

            return base.AwakeHandle(request);
        }

        private void Update()
        {
            if (terrainChunks.ChunkArray.Length <= 1) return;

            ScrollMap();
        }

        public void DefineMapBorders()
        {
            CalculateMapGridSize();
            mapBorderFinder.FindMapBoundaryIndexes(mapGridCreator.ChunkDistance, mapGridEdgeSize);
            mapBorderFinder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        /// <summary>
        /// Checks wether target object exceeds compared limits to trigger chunk shifting.
        /// </summary>
        private void ScrollMap()
        {
            float halfDistance = mapGridCreator.ChunkDistance * (terrainAttributes.RenderChunkSize);
            if (targetObject.position.x < mapBorderFinder.LeftChunk.PositionWorldSpace.x + halfDistance) // Reposition Left
            {
                RepositionColToLeft();
            }
            else if (targetObject.position.x > mapBorderFinder.RightChunk.PositionWorldSpace.x - halfDistance / 2)// - (chunkDistance / 2) * characteristics.MapSize)) // Reposition Right
            {
                RepositionColToRight();
            }
            else if (targetObject.position.z > mapBorderFinder.TopChunk.PositionWorldSpace.z - halfDistance / 2) // Reposition Up
            {
                RepositionRowToTop();
            }
            else if (targetObject.position.z < mapBorderFinder.BottomChunk.PositionWorldSpace.z + halfDistance) // Reposition Down
            {
                RepositionRowToBottom();
            }
        }


        private void RepositionColToLeft()
        {
            ShiftAndEnumerateHorizontalCol(mapBorderFinder.RightmostEdgeCol, -1);

            mapBorderFinder.LeftMostEdgeCol = mapBorderFinder.RightmostEdgeCol;
            mapBorderFinder.RightmostEdgeCol--;

            if (mapBorderFinder.RightmostEdgeCol < 0)
            {
                mapBorderFinder.RightmostEdgeCol = mapGridCreator.ChunkDistance * 2;
            }

            mapBorderFinder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        private void RepositionColToRight()
        {
            ShiftAndEnumerateHorizontalCol(mapBorderFinder.LeftMostEdgeCol, 1);

            mapBorderFinder.RightmostEdgeCol = mapBorderFinder.LeftMostEdgeCol;
            mapBorderFinder.LeftMostEdgeCol++;

            if (mapBorderFinder.LeftMostEdgeCol == mapGridEdgeSize)
            {
                mapBorderFinder.LeftMostEdgeCol = 0;
            }

            mapBorderFinder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
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
                newPosition.x += mapGridEdgeSize * (terrainAttributes.RenderChunkSize) * movementDirection;
                terrainChunks.ChunkArray[index].PositionWorldSpace = newPosition;
                terrainRunner.ProcessChunk(terrainChunks.ChunkArray[index]);
            }
        }

        private void RepositionRowToTop()
        {
            ShiftAndEnumerateVerticalRow(mapBorderFinder.BottomMostEdgeRow, 1);

            mapBorderFinder.TopMostEdgeRow = mapBorderFinder.BottomMostEdgeRow;
            mapBorderFinder.BottomMostEdgeRow--;

            if (mapBorderFinder.BottomMostEdgeRow < 0)
            {
                mapBorderFinder.BottomMostEdgeRow = mapGridCreator.ChunkDistance * 2;
            }

            mapBorderFinder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
        }

        private void RepositionRowToBottom()
        {
            ShiftAndEnumerateVerticalRow(mapBorderFinder.TopMostEdgeRow, -1);

            mapBorderFinder.BottomMostEdgeRow = mapBorderFinder.TopMostEdgeRow;
            mapBorderFinder.TopMostEdgeRow++;

            if (mapBorderFinder.TopMostEdgeRow == mapGridEdgeSize)
            {
                mapBorderFinder.TopMostEdgeRow = 0;
            }

            mapBorderFinder.DefineReferenceChunksInCardinalDirections(terrainChunks.ChunkArray, mapGridEdgeSize);
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
                newPosition.z += terrainAttributes.RenderChunkSize * mapGridEdgeSize * movementDirection;
                terrainChunks.ChunkArray[index].PositionWorldSpace = newPosition;
                terrainRunner.ProcessChunk(terrainChunks.ChunkArray[index]);
            }
        }

        public void CalculateMapGridSize() => mapGridEdgeSize = mapGridCreator.ChunkDistance * 2 + 1;

        #endregion Methods
    }
}
