using MiniProceduralGeneration.ComputeShaders.Processors;
using UnityEngine;

namespace MiniProceduralGeneration.Noise.Processor
{

    public class NoiseProcessor : BaseProcessor, INoiseProcessor
    {

        #region - - - - - - Fields - - - - - -

        private INoiseAttributes noiseCharacteristics;
        private NoiseComputeBuffers computeBuffers = new NoiseComputeBuffers();

        private float[] noiseData;
        private int mapSize;
        private Vector3 samplePosition;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public void Awake()
        {
            noiseCharacteristics = this.GetComponent<INoiseAttributes>();
        }

        public void ProcessNoiseData(float[] noiseDataArray, int mapSize, Vector3 samplePosition)
        {
            this.noiseData = noiseDataArray;
            this.mapSize = mapSize;
            this.samplePosition = samplePosition;

            CreateShaderBuffers();
            SetComputeShaderData();
            shaderProcessor.Dispatch(0, noiseDataArray.Length / 10, 1, 1); // Runs compute shader
            computeBuffers.noiseBuffer.GetData(noiseDataArray);

            // cleans buffers before next use.
            ReleaseBuffersToGarbageCollection();
        }

        protected override void CreateShaderBuffers()
        {
            computeBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float));
            computeBuffers.noiseBuffer.SetData(noiseData);

            computeBuffers.offsetBuffer = new ComputeBuffer(noiseCharacteristics.StepOffsets.Length, sizeof(float) * 2);
            computeBuffers.offsetBuffer.SetData(noiseCharacteristics.StepOffsets);
        }

        protected override void SetComputeShaderData()
        {
            shaderProcessor.SetBuffer(0, "noise", computeBuffers.noiseBuffer);
            shaderProcessor.SetBuffer(0, "octaveOffsets", computeBuffers.offsetBuffer);
            shaderProcessor.SetVector("startPosition", samplePosition);
            shaderProcessor.SetFloat("noiseScale", noiseCharacteristics.NoiseScale);
            shaderProcessor.SetFloat("persistence", noiseCharacteristics.Persistence);
            shaderProcessor.SetFloat("lacunarity", noiseCharacteristics.Lacunarity);
            shaderProcessor.SetInt("chunkSize", mapSize);
            shaderProcessor.SetInt("noiseOctaveCount", (int)noiseCharacteristics.NoiseOctaveCount);
        }

        protected override void ReleaseBuffersToGarbageCollection()
        {
            computeBuffers.noiseBuffer.Release();
            computeBuffers.offsetBuffer.Release();
        }

        #endregion Methods
    }

    public struct NoiseComputeBuffers
    {
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer offsetBuffer;
    }
}
