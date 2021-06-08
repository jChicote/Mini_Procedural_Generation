using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaderTerrainGeneration
{
    public struct MeshPointData
    {
        public Vector3 vert;
        public Vector3 normal;
        public Vector2 uv;
    }

    public class TerrainGenerator : MonoBehaviour
    {
        public ComputeShader computeTerrainGen;
        public NoiseGenerator noiseGenerator;

        [Header("Terrain Cahracteristics")]
        public int resolution;
        public float maxHeight = 10;
        public int groundlevel;
        public int width = 2; // aspect will be 1:1

        public GameObject prefab;

        private List<GameObject> cubeObjects;
        private MeshPointData[] data;

        public void BuildTerrain()
        {
            noiseGenerator.GenerateNoiseSeed();
            float[] noiseData = noiseGenerator.CalculateNoise(width, new Vector3(0,0,0));

            int totalSize = sizeof(float) * 2 + sizeof(float) * 6; // size of the data (4 bytes for each float)
            ComputeBuffer meshBuffer = new ComputeBuffer(data.Length, totalSize);
            meshBuffer.SetData(data);

            int noiseSize = sizeof(float); // size of the data (4 bytes for each float)
            ComputeBuffer noiseBuffer = new ComputeBuffer(noiseData.Length, noiseSize);
            noiseBuffer.SetData(noiseData);

            computeTerrainGen.SetBuffer(0, "meshPoints", meshBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", noiseBuffer);
            computeTerrainGen.SetFloat("resolution", data.Length);
            computeTerrainGen.SetFloat("maxHeight", maxHeight);
            computeTerrainGen.Dispatch(0, data.Length / 10, 1, 1);

            meshBuffer.GetData(data);

            for (int i = 0; i < cubeObjects.Count; i++)
            {
                GameObject obj = cubeObjects[i];
                MeshPointData pointData = data[i];
                obj.transform.position = pointData.vert;
            }

            meshBuffer.Dispose();
        }

        private void PopulateMesh()
        {
            cubeObjects = new List<GameObject>();
            data = new MeshPointData[width * width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Vector3 newPos = new Vector3(i, 0, j);
                    GameObject cube = Instantiate(prefab, transform.position + newPos, Quaternion.identity);
                    cubeObjects.Add(cube);

                    MeshPointData pointData = new MeshPointData
                    {
                        vert = newPos,
                        normal = Vector3.one,
                        uv = Vector2.zero
                    };

                    data[(i * width) + j] = pointData;
                }
            }
        }

        public void RenderTerrain()
        {

        }

        private void OnGUI()
        {
            if (cubeObjects == null)
            {
                if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
                {
                    PopulateMesh();
                    BuildTerrain();
                }
            }

            if (cubeObjects != null)
            {
                if (GUI.Button(new Rect(120, 0, 100, 50), "Regenerate"))
                {
                    BuildTerrain();
                }
            }
        }
    }
}
