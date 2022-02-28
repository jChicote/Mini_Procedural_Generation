using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    /// <summary>
    /// Interfaces for accessing terrain chunks properties for modifying
    /// mesh information.
    /// </summary>
    public interface IChunkMeshAttributes
    {

        #region - - - - - - Properties - - - - - -

        Vector3[] Vertices { get; set; }
        Vector3[] Normals { get; set; }
        Vector2[] UVs { get; set; }
        int[] Triangles { get; set; }

        #endregion Properties

    }

}
