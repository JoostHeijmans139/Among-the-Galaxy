using UnityEngine;
using Random = System.Random;

/// <summary>
/// Helper class providing utility methods for noise generation, such as generating octave offsets and normalizing noise maps.
/// </summary>
public static class NoiseHelper
{
    /// <summary>
    /// Generates an array of random 2D offsets for each octave, used to vary Perlin noise sampling and reduce tiling artifacts.
    /// </summary>
    /// <param name="octaves">Number of octaves (layers) for which to generate offsets.</param>
    /// <param name="offsets">Global offset applied to each generated octave offset.</param>
    /// <param name="seed">Value added to the noise to make the offsets more random</param>
    /// <returns>An array of Vector2 offsets, one for each octave.</returns>
    public static Vector2[] GenerateOffsets(int octaves, Vector2 offsets,int seed)
    {
        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        // Generate a random offset for each octave to ensure unique sampling positions.
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offsets.x;
            float offsetY = rand.Next(-100000, 100000) + offsets.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    /// <summary>
    /// Normalizes the values in a 2D noise map to the \[0,1\] range based on provided minimum and maximum noise heights.
    /// </summary>
    /// <param name="noiseMap">A 2D float array of unnormalized noise values.</param>
    /// <param name="minNoiseHeight">The minimum noise value found in the map.</param>
    /// <param name="maxNoiseHeight">The maximum noise value found in the map.</param>
    /// <param name="mapWidth">Width of the noise map.</param>
    /// <param name="mapHeight">Height of the noise map.</param>
    /// <returns>A 2D float array with all values normalized to the \[0,1\] range.</returns>
    public static float[,] NormalizeNoiseMap(float[,] noiseMap, float minNoiseHeight, float maxNoiseHeight, int mapWidth, int mapHeight)
    {
        // Iterate through each value and normalize using Mathf.InverseLerp.
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
