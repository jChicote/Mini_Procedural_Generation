using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniProceduralGeneration.Generator;

namespace MiniProceduralGeneration.Utility.Visualisation
{
    public class NoiseVisualiser : MonoBehaviour
    {
        public NoiseGenerator noiseGenerator;
        public TerrainManager terrainGenerator;
        private Image resultImage;
        private int imageWidth;
        private int imageHeight;

        private void Awake()
        {
            resultImage = this.GetComponent<Image>();

            GetImageDimensions();
        }

        public void CreateVisualisation()
        {
            Texture2D texture = GenerateImage();
            Sprite visualisation = Sprite.Create(texture, new Rect(0, 0, terrainGenerator.RenderChunkSize, terrainGenerator.RenderChunkSize), new Vector2(0.5f, 0.5f), 100f);

            resultImage.sprite = visualisation;
        }

        private void GetImageDimensions()
        {
            RectTransform resultTransform = this.GetComponent<RectTransform>();
            imageWidth = (int)resultTransform.rect.width;
            imageHeight = (int)resultTransform.rect.height;
        }

        private Texture2D GenerateImage()
        {
            Texture2D texture = new Texture2D(imageWidth, imageHeight);
            GetImageDimensions();
            texture.Resize(terrainGenerator.RenderChunkSize, terrainGenerator.RenderChunkSize);

            for (int index = 0, row = 0; row < terrainGenerator.RenderChunkSize; row++)
            {
                for (int col = 0; col < terrainGenerator.RenderChunkSize; col++, index++)
                {
                    Color pixel = new Color(noiseGenerator.NoiseData[index], noiseGenerator.NoiseData[index], noiseGenerator.NoiseData[index]);
                    texture.SetPixel(col, row, pixel);
                }
            }

            texture.Apply();
            return texture;
        }

    }
}
