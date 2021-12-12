using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniProceduralGeneration.UserInterfaces
{
    /// <summary>
    /// Class for providing toggle control on Terrain generator panels.
    /// </summary>
    public class TerrainGeneratorUIControls : MonoBehaviour
    {
        // Fields
        public GameObject terrainPanel;
        public GameObject noisePanel;
        public Button terrainButton;
        public Button noiseButton;

        public void ToggleTerrainPanel()
        {

            noisePanel.SetActive(false);
            terrainPanel.SetActive(true);
        }

        public void ToggleNoisePanel()
        {
            terrainPanel.SetActive(false);
            noisePanel.SetActive(true);
        }

        public void EnableInteraction()
        {
            terrainButton.interactable = true;
            noiseButton.interactable = true;
        }
    }
}
