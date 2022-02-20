using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver;
using MiniProceduralGeneration.TerrainCore.Processor;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore
{

    public class TerrainChunkIterator : MonoBehaviour, ITerrainChunkIterator
    {

        #region - - - - Fields - - - -

        private float[] noiseData;
        private INoiseGenerator noiseGenerator;
        private ITerrainAttributes attributes;
        private IMeshTerrainProcessor terrainProcessor;

        #endregion Fields

        #region - - - - Constructors - - - -

        public void StartTerrainRunnerAction(ITerrainAttributes attributes, IMeshTerrainProcessor terrainProcessor, INoiseGenerator noiseGenerator)
        {
            this.attributes = attributes;
            this.terrainProcessor = terrainProcessor;
            this.noiseGenerator = noiseGenerator;
        }

        #endregion Constructors

        #region - - - - Methods - - - -

        public void IterateThroughChunkArraySelection(IChunkShell[] chunks)
        {
            foreach (IChunkShell chunk in chunks)
            {
                ProcessChunk(chunk, false);
            }
        }

        public void ProcessChunk(IChunkShell chunk, bool forceUpdate)
        {
            if (noiseData is null)
                _ = new float[0];

            ChunkDimensionsResolver resolver = this.GetComponent<ChunkDimensionsResolver>();
            TerrainChunkDimensions dimensions = resolver.GetChunkDimensions(chunk.PositionWorldSpace);

            //print(chunk.Dimensions != null && forceUpdate);
            //if (chunk.Dimensions != null)
            //{
            //    print("Current LOD: " + dimensions.LevelOfDetail + ", Minimum LOD: " + resolver.MinimumLevelOfDetail);
            //    if (chunk.Dimensions.LevelOfDetail == resolver.MinimumLevelOfDetail)
            //    {
            //        print("Worked a number of times");
            //    }
            //}

            // Render If it is forced to update

            if (forceUpdate || chunk.Dimensions == null)
            {
                chunk.InitialiseMeshArrays(dimensions);

                this.noiseData = noiseGenerator.SampleNoiseDataAtLocation(attributes.ActualChunkSize, chunk.PositionWorldSpace);
                terrainProcessor.ProcessChunkMesh(chunk, noiseData);
                chunk.BuildMesh();
            }
            else
            {
                if (chunk.Dimensions.LevelOfDetail == dimensions.LevelOfDetail)
                {
                    //print("Local: " + chunk.Dimensions.LevelOfDetail + "Minimum: " + resolver.MinimumLevelOfDetail);
                    return;
                }

                chunk.InitialiseMeshArrays(dimensions);

                this.noiseData = noiseGenerator.SampleNoiseDataAtLocation(attributes.ActualChunkSize, chunk.PositionWorldSpace);
                terrainProcessor.ProcessChunkMesh(chunk, noiseData);
                chunk.BuildMesh();
            }

        }

        #endregion Methods

    }

}
