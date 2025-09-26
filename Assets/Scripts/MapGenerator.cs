using UnityEngine;

/// <summary>
/// Generates a procedural map using Perlin noise and displays it using different modes.
/// Supports drawing noise maps, colored maps, or mesh representations.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Modes for how the map will be drawn/displayed.
    /// </summary>
    public enum DrawMode
    {
        DrawNoiseMap,   // Display the raw noise values as a grayscale image
        DrawColorMap,   // Display the noise map colored by terrain types
        DrawMesh,       // Display the map as a 3D mesh with textures
    }

    [Header("Map Settings")]
    public DrawMode drawMode;        // Current mode for displaying the map
    public int mapWidth;             // Width of the map in units
    public int mapHeight;            // Height of the map in units

    [Header("Noise Settings")]
    public int seed = 0;
    [Range(2, 100)]
    public float noiseScale;         // Scale of the noise (affects zoom)
    [Range(0, 20)]
    public int octaves;              // Number of noise layers combined
    [Range(0, 1)]
    public float persistence;        // Controls amplitude of octaves (affects roughness)
    public float lacunarity;         // Controls frequency of octaves (affects detail)
    public float heightMultiplier;
    public Vector2 offsets;
    [Header("Other Settings")]
    public bool autoUpdate;          // If true, map auto regenerates when settings change
    public TerrainType[] TerrainTypes; // Array defining different terrain types by height and color

    /// <summary>
    /// Generates the map data (noise map) and displays it based on the selected draw mode.
    /// </summary>
    public void GenerateMap()
    {
        // Generate the noise map based on the current parameters
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistence, lacunarity,offsets);

        // Find the DisplayMap component in the scene to show the map
        DisplayMap display = FindObjectOfType<DisplayMap>();

        // Generate a color map by assigning colors to each noise value based on terrain height
        Color[] colorMap = GenerateColorMap(noiseMap, TerrainTypes);

        // Display the map according to the selected draw mode
        switch (drawMode)
        {
            case DrawMode.DrawNoiseMap:
                display.DisplayNoiseMap(noiseMap);
                break;

            case DrawMode.DrawColorMap:
                display.DisplayColorMap(colorMap, mapWidth, mapHeight);
                break;

            case DrawMode.DrawMesh:
                // Generate mesh from noise map and texture from color map
                display.DrawMesh(
                    MeshGenerator.GenerateTerainMesh(noiseMap,heightMultiplier),
                    TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight)
                );
                break;
        }
    }

    /// <summary>
    /// Converts a noise map into a color map by mapping noise values to terrain types.
    /// </summary>
    /// <param name="noiseMap">2D array of noise values between 0 and 1.</param>
    /// <param name="terrainTypes">Array of terrain types with associated heights and colors.</param>
    /// <returns>1D array of colors representing the map's color data.</returns>
    private static Color[] GenerateColorMap(float[,] noiseMap, TerrainType[] terrainTypes)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Current noise value representing height at (x, y)
                float currentHeight = noiseMap[x, y];

                // Map the current height to a terrain type color
                for (int i = 0; i < terrainTypes.Length; i++)
                {
                    if (currentHeight <= terrainTypes[i].Height)
                    {
                        colorMap[y * mapWidth + x] = terrainTypes[i].Color;
                        break; // Exit loop after finding matching terrain type
                    }
                }
            }
        }

        return colorMap;
    }

    /// <summary>
    /// Unity Editor callback to validate and clamp variables when they are changed in the Inspector.
    /// Ensures that map dimensions and noise parameters stay within acceptable ranges.
    /// </summary>
    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1; // Ensure minimum map width
        }

        if (mapHeight < 1)
        {
            mapHeight = mapWidth; // Keep height consistent if less than 1
        }

        if (lacunarity < 1)
        {
            lacunarity = 1; // Lacunarity should be at least 1
        }

        if (octaves < 0)
        {
            octaves = 0; // Octaves cannot be negative
        }
    }

    /// <summary>
    /// Struct representing a terrain type with a name, color, and height threshold.
    /// </summary>
    [System.Serializable]
    public struct TerrainType
    {
        public string Name;   // Name of the terrain type (e.g., Water, Sand, Grass)
        public Color Color;   // Color used to represent this terrain on the map
        public float Height;  // Maximum height value that maps to this terrain type
    }
}
