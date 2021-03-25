using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProceduralGeneration.TerrainGeneration;

namespace ProceduralGeneration.InspectorEditor
{
    [CustomEditor(typeof(OfflineTerrainGenerator))]
    public class TerrainGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            OfflineTerrainGenerator landGenerator = (OfflineTerrainGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                landGenerator.GenerateMap();
            }
        }
    }
}
