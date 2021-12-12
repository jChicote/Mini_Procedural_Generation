using UnityEngine;
using UnityEngine.UIElements;
using MiniProceduralGeneration.Generator;

namespace MiniProceduralGeneration.UserInterfaces
{
    public class GUITerrainChunkControls : MonoBehaviour
    {
        private ITerrainGenerator terrainGenerator;
        private ITerrainCharacteristics terrainCharacteristics;

        [Header("Sliders")]
        public Slider maxHeightSlider;

        private void Start()
        {
            TerrainGenerator generator = FindObjectOfType<TerrainGenerator>();
            terrainCharacteristics = generator;
            terrainGenerator = generator;



            //maxHeightSlider.value = terrainCharacteristics.MaxHeight;
        }

        private void OnGUI()
        {
            //hSbarValue = GUILayout.HorizontalScrollbar(hSbarValue, 10.0f, 10.0f, 70.0f);
            

            /*if (maxHeightSlider.value != terrainCharacteristics.MaxHeight)
            {
                terrainCharacteristics.MaxHeight = maxHeightSlider.value;
                terrainGenerator.CalculateChunkDimensions();
                terrainGenerator.InitialiseTerrainChunks();
                terrainGenerator.BuildTerrain();
            }*/

        }
    }
}
