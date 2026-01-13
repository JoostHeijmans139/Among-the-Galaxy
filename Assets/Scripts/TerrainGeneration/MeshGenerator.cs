using JetBrains.Annotations;
using UnityEngine;

namespace TerrainGeneration
{
 public static class MeshGenerator
{
    /// <summary>
    /// Static class responsible for generating terrain meshes from height maps.
    /// </summary>
    /// <summary>
    /// Generates a MeshData object representing a terrain mesh based on a 2D height map.
    /// </summary>
    /// <param name="heighMap">2D array of height values for each vertex.</param>
    /// <param name="heightMultiplier">Multiplier applied to height values for vertical scaling.</param>
    /// <param name="heightCurve">AnimationCurve to shape the height distribution.</param>
    /// <param name="levelOfDetail">Level of detail for mesh simplification (0 = highest detail).</param>
    /// <returns>MeshData containing vertices, triangles, and UVs for the terrain mesh.</returns>
    /// <returns>MeshData containing vertices, triangles, and UVs representing the terrain mesh.</returns>
    public static MeshData GenerateTerrainMesh(float[,] heighMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        AnimationCurve Heightcurve = new AnimationCurve(heightCurve.keys);
        int width = heighMap.GetLength(0);
        int height = heighMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
    
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        int vertexIndex = 0;
        int yIndex = 0;  // Track simplified Y position

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            int xIndex = 0;  // Track simplified X position
        
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                float heightValue = Mathf.Clamp01(Heightcurve.Evaluate(heighMap[x, y]));
                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightValue * heightMultiplier, topLeftZ - y);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Only create triangles if NOT on the last row or column of simplified mesh
                if (xIndex < verticesPerLine - 1 && yIndex < verticesPerLine - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
                xIndex++;
            }
            yIndex++;
        }
    
        SmoothMeshEdges(meshData, verticesPerLine);
        return meshData;
    }
    // Add to GenerateTerrainMesh before returning meshData:
    private static void SmoothMeshEdges(MeshData meshData, int verticesPerLine, float smoothingFactor = 0.5f)
    {
        for (int i = 0; i < verticesPerLine; i++) {
            // Smooth left edge
            if (i > 0 && i < verticesPerLine - 1) {
                Vector3 edge = meshData.Vertices[i];
                Vector3 prev = meshData.Vertices[i - 1];
                Vector3 next = meshData.Vertices[i + 1];
                meshData.Vertices[i] = Vector3.Lerp(edge, (prev + next) * 0.5f, smoothingFactor);
            }
            
            // Smooth right edge
            int rightEdge = meshData.Vertices.Length - 1 - i;
            if (i > 0 && i < verticesPerLine - 1) {
                Vector3 edge = meshData.Vertices[rightEdge];
                Vector3 prev = meshData.Vertices[rightEdge - 1];
                Vector3 next = meshData.Vertices[rightEdge + 1];
                meshData.Vertices[rightEdge] = Vector3.Lerp(edge, (prev + next) * 0.5f, smoothingFactor);
            }
        }
    }
}

/// <summary>
/// Class that stores mesh data such as vertices, triangles, and UVs, and provides methods to build a Unity Mesh.
/// </summary>
public class MeshData
{
    public readonly Vector3[] Vertices;   // Array of vertex positions
    public readonly int[] Triangles;      // Array of vertex indices defining mesh triangles
    private int _triangleIndex;   // Current index position for inserting triangles
    public readonly Vector2[] Uvs;        // Array of UV coordinates for texturing
    [CanBeNull] public Texture2D Colormap;

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
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    /// <summary>
    /// Adds a triangle to the triangles array by specifying the three vertex indices.
    /// </summary>
    /// <param name="a">Index of the first vertex.</param>
    /// <param name="b">Index of the second vertex.</param>
    /// <param name="c">Index of the third vertex.</param>
    public void AddTriangle(int a, int b, int c)
    {
        Triangles[_triangleIndex] = a;
        Triangles[_triangleIndex + 1] = b;
        Triangles[_triangleIndex + 2] = c;
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
            triangles = Triangles,
            uv = Uvs
        };

        mesh.RecalculateBounds();  // Recalculate bounding box of the mesh
        mesh.RecalculateNormals(); // Recalculate normals for lighting

        return mesh;
    }
}   
}
