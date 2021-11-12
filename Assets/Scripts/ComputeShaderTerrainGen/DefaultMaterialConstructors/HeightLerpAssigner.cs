using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaderTerrainGeneration
{
    public class HeightLerpAssigner : MonoBehaviour
    {
        public void AssignLerpColors(float maxHeight, float minHeight)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            Material sharedMat = renderer.sharedMaterial;
            sharedMat.SetFloat("_MaxHeight", maxHeight);
            sharedMat.SetFloat("_MinHeight", minHeight);
        }
    }
}
