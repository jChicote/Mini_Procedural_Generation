using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLandGenerator : MonoBehaviour
{
    // Insepector accessible fields
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    [SerializeField] private float width;
    [SerializeField] private float height;

    // Fields
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uv;

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        meshCollider = this.GetComponent<MeshCollider>();
        vertices = new Vector3[4];
        normals = new Vector3[4];
        uv = new Vector2[4];

        GenerateBasicQuad();
    }

    /// <summary>
    /// Generates basic quad for testing purposes
    /// </summary>
    public void GenerateBasicQuad()
    {
        Mesh mesh = new Mesh();

        vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, 0, height),
            new Vector3(width, 0, height)
        };
        mesh.vertices = vertices;

        // The tris is the triangular geometry calculated within the quad
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void GenerateFlatTerrain()
    {

    }
}
