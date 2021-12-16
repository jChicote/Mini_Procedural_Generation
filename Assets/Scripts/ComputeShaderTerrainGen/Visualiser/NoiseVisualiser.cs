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
        private Image resultImage;
        private int imageWidth;
        private int imageHeight;

        private void Awake()
        {
            resultImage = this.GetComponent<Image>();

            RectTransform resultTransform = this.GetComponent<RectTransform>();
            imageWidth = (int)resultTransform.rect.width;
            imageHeight = (int)resultTransform.rect.height;
        }

        public void CreateVisualisation()
        {
            Texture2D texture = GenerateImage();
            Sprite visualisation = Sprite.Create(texture, new Rect(0, 0, imageWidth, imageHeight), new Vector2(0.5f, 0.5f), 100f);

            resultImage.sprite = visualisation;
        }

        private Texture2D GenerateImage()
        {
            Texture2D texture = new Texture2D(240, 240);
            //float[] noiseData = noiseGenerator.NoiseData;

            for (int index = 0, row = 0; row < 240; row++)
            {
                for (int col = 0; col < 240; col++, index++)
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
