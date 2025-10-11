using UnityEngine;

/// <summary>
/// Static class responsible for generating terrain meshes from height maps.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Generates a MeshData object representing a terrain mesh based on a 2D height map.
    /// </summary>
    /// <param name="heighMap">2D array of height values for each vertex.</param>
    /// <param name="heightMultiplier">Multiplier applied to height values for vertical scaling.</param>
    /// <param name="heightCurve">AnimationCurve to shape the height distribution.</param>
    /// <param name="levelOfDetail">Level of detail for mesh simplification (0 = highest detail).</param>
    /// <returns>MeshData containing vertices, triangles, and UVs for the terrain mesh.</returns>
    /// <returns>MeshData containing vertices, triangles, and UVs representing the terrain mesh.</returns>
    public static MeshData GenerateTerrainMesh(float[,] heighMap,float heightMultiplier,AnimationCurve heightCurve, int levelOfDetail)
    {
        AnimationCurve Heightcurve = new  AnimationCurve(heightCurve.keys);
        int width = heighMap.GetLength(0);  // Number of vertices along X-axis
        int height = heighMap.GetLength(1); // Number of vertices along Z-axis

        // Calculate the top-left corner position to center the mesh around origin
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        
        int meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        
        // Create a new MeshData object to hold mesh info
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        int vertexIndex = 0; // Tracks the current vertex index while iterating

        // Loop through each vertex coordinate in the height map grid
        for (int y = 0; y < height; y+=meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x+=meshSimplificationIncrement)
            {
                // Assign the vertex position:
                // X = topLeftX offset + x position
                // Y = height value from height map
                // Z = topLeftZ offset - y position (negative because Unity's Z axis is forward)
                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, Heightcurve.Evaluate(heighMap[x, y])*heightMultiplier , topLeftZ - y);

                // Calculate UV coordinates for texturing (range 0 to 1)
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Generate triangles for all vertices except those on the far right and bottom edges
                if (x < width - 1 && y < height - 1)
                {
                    // Each square consists of two triangles; add their indices in clockwise order
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++; // Move to next vertex index
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
    public readonly Vector3[] Vertices;   // Array of vertex positions
    private readonly int[] _triangles;      // Array of vertex indices defining mesh triangles
    private int _triangleIndex;   // Current index position for inserting triangles
    public readonly Vector2[] Uvs;        // Array of UV coordinates for texturing

    /// <summary>
    /// Constructs the MeshData object with allocated arrays based on mesh dimensions.
    /// </summary>
    /// <param name="meshWidth">Width (number of vertices) of the mesh.</param>
    /// <param name="meshHeight">Height (number of vertices) of the mesh.</param>
    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        Uvs = new Vector2[meshWidth * meshHeight];

        // Each square (quad) consists of 2 triangles, each triangle has 3 vertices
        // Number of quads = (meshWidth-1)*(meshHeight-1)
        _triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    /// <summary>
    /// Adds a triangle to the triangles array by specifying the three vertex indices.
    /// </summary>
    /// <param name="a">Index of the first vertex.</param>
    /// <param name="b">Index of the second vertex.</param>
    /// <param name="c">Index of the third vertex.</param>
    public void AddTriangle(int a, int b, int c)
    {
        _triangles[_triangleIndex] = a;
        _triangles[_triangleIndex + 1] = b;
        _triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3; // Move to next available slot for triangles
    }

    /// <summary>
    /// Creates a Unity Mesh object from the stored mesh data.
    /// </summary>
    /// <returns>A UnityEngine.Mesh constructed from vertices, triangles, and UVs.</returns>
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh()
        {
            vertices = Vertices,
            triangles = _triangles,
            uv = Uvs
        };
        mesh.vertices = Vertices;
        mesh.triangles = _triangles;
        mesh.uv = Uvs;

        mesh.RecalculateBounds();  // Recalculate bounding box of the mesh
        mesh.RecalculateNormals(); // Recalculate normals for lighting

        return mesh;
    }
}
