using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides methods for generating 2D Perlin noise maps, used in procedural terrain generation.
/// </summary>
public static class Noise
{
    public enum NormalizedMode
    {
        Local,
        Global,
    }
    public static NormalizedMode normalizedMode;
    public static float maxPossibleHeight = 0f;
    public static float amplitude = 1f;
    public static float persistence;
    /// <summary>
    /// Generates a 2D array of Perlin noise values using multiple octaves, suitable for terrain heightmaps.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="seed">Random seed for reproducibility (currently unused).</param>
    /// <param name="scale">Controls the zoom level of the noise. Higher values produce smoother terrain.</param>
    /// <param name="octaves">Number of noise layers (octaves) to combine for detail.</param>
    /// <param name="persistence">Controls amplitude reduction per octave (affects roughness).</param>
    /// <param name="lacunarity">Controls frequency increase per octave (affects detail scale).</param>
    /// <param name="offsets">Global offset applied to all noise samples.</param>
    /// <returns>A 2D float array representing the generated noise map.</returns>
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        int seed,
        float scale,
        int octaves,
        float persistence,
        float lacunarity,
        Vector2 offsets,NormalizedMode normalizedMode)
    {
        Noise.persistence = persistence;
        Noise.normalizedMode = normalizedMode;
        float[,] noiseMap = new float[width, height];

        // Track the minimum and maximum noise values for normalization.
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;
        float frequency = 1f;
        // Generate per-octave offsets for more varied noise.
        Vector2[] octaveOffsets = NoiseHelper.GenerateOffsets(octaves, offsets,seed);

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        // Iterate over each coordinate in the noise map.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1f;
                frequency = 1f;
                float noiseHeight = 0f;

                // Combine multiple octaves of Perlin noise.
                for (int i = 0; i < octaves; i++)
                {
                    // Calculate sample coordinates with scaling, frequency, and offset.
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

                    // Perlin noise returns [0,1], remap to [-1,1].
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += noiseValue * amplitude;

                    // Update amplitude and frequency for next octave.
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Track min and max for normalization.
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map to the [0,1] range.
        noiseMap = NoiseHelper.NormalizeNoiseMap(noiseMap, minNoiseHeight, maxNoiseHeight, width, height);

        return noiseMap;
    }
}
