using UnityEngine;
using MiniProceduralGeneration.Generator;

namespace MiniProceduralGeneration.Generator.Processor
{
    public interface INoiseProcessor
    {
        void ProcessNoiseData(float[] noiseDataArray, int mapSize, Vector3 samplePosition);
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoiseProcessor : MonoBehaviour, INoiseProcessor
    {
        // Fields
        [SerializeField] private ComputeShader noiseShader;
        private INoiseCharacteristics noiseCharacteristics;
        private NoiseComputeBuffers computeBuffers = new NoiseComputeBuffers();

        private void Awake()
        {
            noiseCharacteristics = this.GetComponent<INoiseCharacteristics>();
        }

        public void ProcessNoiseData(float[] noiseDataArray, int mapSize, Vector3 samplePosition)
        {
            CreateNoiseComputeBuffers(noiseDataArray);

            // Produce noise data through compute shader
            SetComputeShaderData(computeBuffers, samplePosition, mapSize);
            noiseShader.Dispatch(0, noiseDataArray.Length / 10, 1, 1); // Runs compute shader
            computeBuffers.noiseBuffer.GetData(noiseDataArray);

            // cleans buffers before next use.
            DisposeBuffersToGarbageCollection(computeBuffers);
        }

        private NoiseComputeBuffers CreateNoiseComputeBuffers(float[] noiseDataArray)
        {
            //computeBuffers = new NoiseComputeBuffers();

            computeBuffers.noiseBuffer = new ComputeBuffer(noiseDataArray.Length, sizeof(float)); 
            computeBuffers.noiseBuffer.SetData(noiseDataArray);

            computeBuffers.offsetBuffer = new ComputeBuffer(noiseCharacteristics.StepOffsets.Length, sizeof(float) * 2); 
            computeBuffers.offsetBuffer.SetData(noiseCharacteristics.StepOffsets);

            return computeBuffers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="computeBuffers"></param>
        /// <param name="startPosition"></param>
        /// <param name="mapSize"></param>
        private void SetComputeShaderData(NoiseComputeBuffers computeBuffers, Vector3 samplePosition, int mapSize)
        {
            noiseShader.SetBuffer(0, "noise", computeBuffers.noiseBuffer);
            noiseShader.SetBuffer(0, "stepOffsets", computeBuffers.offsetBuffer);
            noiseShader.SetVector("startPosition", samplePosition);
            noiseShader.SetFloat("noiseScale", noiseCharacteristics.NoiseScale);
            noiseShader.SetFloat("persistence", noiseCharacteristics.Persistence);
            noiseShader.SetFloat("lacunarity", noiseCharacteristics.Lacunarity);
            noiseShader.SetInt("mapDimension", mapSize);
            noiseShader.SetInt("stepDetailCount", noiseCharacteristics.StepDetailCount);
        }

        private void DisposeBuffersToGarbageCollection(NoiseComputeBuffers computeBuffers)
        {
            computeBuffers.noiseBuffer.Release();
            computeBuffers.offsetBuffer.Release();
        }
    }

    public struct NoiseComputeBuffers
    {
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer offsetBuffer;
    }
}
