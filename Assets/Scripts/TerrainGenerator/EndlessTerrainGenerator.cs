using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralGeneration.MeshGeneration;
using ProceduralGeneration.NoiseGeneration;

namespace ProceduralGeneration.TerrainGeneration
{
    public class EndlessTerrainGenerator : MonoBehaviour
    {
        public const float maxViewDistance = 300;
        public Transform viewer;
        public GameObject meshPrefab;
        public NoiseGenerator noiseGenerator;

        public static Vector2 viewerPosition;
        private int chunkSize;
        private int chunkVisibleInViewDist;

        private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

        private void Start()
        {
            chunkSize = MeshGenerator.mapSize - 1;
            chunkVisibleInViewDist = Mathf.RoundToInt(maxViewDistance / chunkSize);
        }

        private void Update()
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
            for (int i = 0; i < terrainChunkVisibleLastUpdate.Count; i++)
            {
                terrainChunkVisibleLastUpdate[i].SetVisible(false);
            }
            terrainChunkVisibleLastUpdate.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int yOffset = -chunkVisibleInViewDist; yOffset <= chunkVisibleInViewDist; yOffset++)
            {
                for (int xOffset = -chunkVisibleInViewDist; xOffset <= chunkVisibleInViewDist; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        if(terrainChunkDictionary[viewedChunkCoord].IsVisible())
                        {
                            terrainChunkVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                        }
                    }
                    else
                    {
                        terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(meshPrefab, noiseGenerator, viewedChunkCoord, chunkSize));
                    }
                }
            }
        }


        public class TerrainChunk
        {
            private GameObject meshObject;
            private Vector2 position;
            Bounds bounds;

            public TerrainChunk(GameObject meshPrefab, NoiseGenerator noiseGenerator, Vector2 coord, int size)
            {
                position = coord * size;
                bounds = new Bounds(position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(position.x, 0, position.y);

                meshObject = GameObject.Instantiate(meshPrefab, positionV3, Quaternion.identity);
                //meshObject.transform.localScale = Vector3.one * size / 10f;
                MeshGenerator meshGenerator = meshObject.GetComponent<MeshGenerator>();
                meshGenerator.NoiseGenerator = noiseGenerator;
                meshGenerator.GenerateBaseMesh(0);
                SetVisible(false);
            }

            public void UpdateTerrainChunk()
            {
                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistFromNearestEdge <= maxViewDistance;
                SetVisible(visible);
            }

            public void SetVisible(bool isVisible)
            {
                meshObject.SetActive(isVisible);
            }

            public bool IsVisible()
            {
                return meshObject.activeSelf;
            }
        }
    }
}
