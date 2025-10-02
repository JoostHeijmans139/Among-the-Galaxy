using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Static class responsible for generating terrain meshes from height maps.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Generates a MeshData object representing a terrain mesh based on a 2D height map.
    /// </summary>
    /// <param name="heighMap">2D float array containing height values for each vertex.</param>
    /// <returns>MeshData containing vertices, triangles, and UVs representing the terrain mesh.</returns>
    public static MeshData GenerateTerainMesh(float[,] heighMap,float heightMultiplier,AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heighMap.GetLength(0);  // Number of vertices along X-axis
        int height = heighMap.GetLength(1); // Number of vertices along Z-axis

        // Calculate the top-left corner position to center the mesh around origin
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        
        // Create a new MeshData object to hold mesh info
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        int vertextIndex = 0; // Tracks the current vertex index while iterating

        // Loop through each vertex coordinate in the height map grid
        for (int y = 0; y < height; y+=meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x+=meshSimplificationIncrement)
            {
                // Assign the vertex position:
                // X = topLeftX offset + x position
                // Y = height value from height map
                // Z = topLeftZ offset - y position (negative because Unity's Z axis is forward)
                meshData.vertices[vertextIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heighMap[x, y])*heightMultiplier , topLeftZ - y);

                // Calculate UV coordinates for texturing (range 0 to 1)
                meshData.uvs[vertextIndex] = new Vector2(x / (float)width, y / (float)height);

                // Generate triangles for all vertices except those on the far right and bottom edges
                if (x < width - 1 && y < height - 1)
                {
                    // Each square consists of two triangles; add their indices in clockwise order
                    meshData.AddTriangle(vertextIndex, vertextIndex + verticesPerLine + 1, vertextIndex + verticesPerLine);
                    meshData.AddTriangle(vertextIndex + verticesPerLine + 1, vertextIndex, vertextIndex + 1);
                }

                vertextIndex++; // Move to next vertex index
            }
        }

        return meshData; // Return the constructed mesh data
    }
}

/// <summary>
/// Class that stores mesh data such as vertices, triangles, and UVs, and provides methods to build a Unity Mesh.
/// </summary>
public class MeshData
{
    public Vector3[] vertices;   // Array of vertex positions
    public int[] triangles;      // Array of vertex indices defining mesh triangles
    private int triangleIndex;   // Current index position for inserting triangles
    public Vector2[] uvs;        // Array of UV coordinates for texturing

    /// <summary>
    /// Constructs the MeshData object with allocated arrays based on mesh dimensions.
    /// </summary>
    /// <param name="meshWidth">Width (number of vertices) of the mesh.</param>
    /// <param name="meshHeight">Height (number of vertices) of the mesh.</param>
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];

        // Each square (quad) consists of 2 triangles, each triangle has 3 vertices
        // Number of quads = (meshWidth-1)*(meshHeight-1)
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    /// <summary>
    /// Adds a triangle to the triangles array by specifying the three vertex indices.
    /// </summary>
    /// <param name="a">Index of the first vertex.</param>
    /// <param name="b">Index of the second vertex.</param>
    /// <param name="c">Index of the third vertex.</param>
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3; // Move to next available slot for triangles
    }

    /// <summary>
    /// Creates a Unity Mesh object from the stored mesh data.
    /// </summary>
    /// <returns>A UnityEngine.Mesh constructed from vertices, triangles, and UVs.</returns>
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateBounds();  // Recalculate bounding box of the mesh
        mesh.RecalculateNormals(); // Recalculate normals for lighting

        return mesh;
    }
}
