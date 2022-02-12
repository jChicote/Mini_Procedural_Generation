using UnityEngine;

namespace MiniProceduralGeneration.ComputeShaders.Processors
{

    public abstract class BaseProcessor : MonoBehaviour
    {

        #region - - - - - - Fields - - - - - -

        public ComputeShader shaderProcessor;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        protected abstract void CreateShaderBuffers();
        protected abstract void SetComputeShaderData();
        protected abstract void ReleaseBuffersToGarbageCollection();

        #endregion Methods

    }

}
