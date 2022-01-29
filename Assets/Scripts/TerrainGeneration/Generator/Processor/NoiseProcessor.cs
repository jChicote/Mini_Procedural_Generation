using UnityEngine;

namespace MiniProceduralGeneration.Generator.Processor
{
    public interface INoiseProcessor
    {
        void ProcessNoiseData(float[] noiseDataArray, int mapSize, Vector3 samplePosition);
    }

    public class NoiseProcessor : MonoBehaviour, INoiseProcessor
    {
        // Fields
        [SerializeField] private ComputeShader noiseShader;
        private INoiseAttributes noiseCharacteristics;
        private NoiseComputeBuffers computeBuffers = new NoiseComputeBuffers();

        public void Awake()
        {
            noiseCharacteristics = this.GetComponent<INoiseAttributes>();
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

        /// <summary>
        /// Creates noise buffers to prepare structured buffers to specified array sizes
        /// and strides.
        /// </summary>
        private void CreateNoiseComputeBuffers(float[] noiseDataArray)
        {
            //computeBuffers = new NoiseComputeBuffers();

            computeBuffers.noiseBuffer = new ComputeBuffer(noiseDataArray.Length, sizeof(float));
            computeBuffers.noiseBuffer.SetData(noiseDataArray);

            computeBuffers.offsetBuffer = new ComputeBuffer(noiseCharacteristics.StepOffsets.Length, sizeof(float) * 2);
            computeBuffers.offsetBuffer.SetData(noiseCharacteristics.StepOffsets);
        }

        /// <summary>
        /// Sets the compute shader to recieve variables of input noise data.
        /// </summary>
        private void SetComputeShaderData(NoiseComputeBuffers computeBuffers, Vector3 samplePosition, int mapSize)
        {
            noiseShader.SetBuffer(0, "noise", computeBuffers.noiseBuffer);
            noiseShader.SetBuffer(0, "octaveOffsets", computeBuffers.offsetBuffer);
            noiseShader.SetVector("startPosition", samplePosition);
            noiseShader.SetFloat("noiseScale", noiseCharacteristics.NoiseScale);
            noiseShader.SetFloat("persistence", noiseCharacteristics.Persistence);
            noiseShader.SetFloat("lacunarity", noiseCharacteristics.Lacunarity);
            noiseShader.SetInt("mapDimension", mapSize);
            print(mapSize);
            noiseShader.SetInt("noiseOctaveCount", (int)noiseCharacteristics.NoiseOctaveCount);
        }

        private void DisposeBuffersToGarbageCollection(NoiseComputeBuffers computeBuffers)
        {
            computeBuffers.noiseBuffer.Dispose();
            computeBuffers.offsetBuffer.Dispose();
        }
    }

    public struct NoiseComputeBuffers
    {
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer offsetBuffer;
    }
}
