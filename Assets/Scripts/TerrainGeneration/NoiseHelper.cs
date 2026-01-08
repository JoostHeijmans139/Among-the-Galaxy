using TerrainGeneration;
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
    public static float[,] NormalizeNoiseMap(float[,] noiseMap, float minNoiseHeight, float maxNoiseHeight, int mapWidth, int mapHeight, Noise.NormalizedMode normalizedMode)
    {
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if (normalizedMode == Noise.NormalizedMode.Local) {
                    float range = maxNoiseHeight - minNoiseHeight;
                    if (range == 0) {
                        noiseMap[x, y] = 0;
                    } else {
                        noiseMap[x, y] = (noiseMap[x, y] - minNoiseHeight) / range;
                    }
                } else {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * Noise.maxPossibleHeight /2.75f);
                    noiseMap[x, y] = Mathf.Clamp01(normalizedHeight);
                }
            }
        }
        return noiseMap;
    }
    public static float[,] SampleNoiseMap(float[,] noiseMap,Vector2 worldPosition,int mapSize = 241,float worldToNoiseScaleBias = 1f)
    {
        float[,] globalNoiseMap = new float[mapSize, mapSize];
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);
        float worldToNoiseScale = mapWidth/worldToNoiseScaleBias;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float worldX = worldPosition.x + x;
                float worldY = worldPosition.y + y;
                int globalX = (Mathf.RoundToInt(worldX*worldToNoiseScale)) % mapWidth;
                int globalY = (Mathf.RoundToInt(worldY*worldToNoiseScale)) % mapHeight;
                
                if (globalX < 0) globalX += mapWidth;
                if (globalY < 0) globalY += mapHeight;
                
                globalNoiseMap[x, y] = BilinearSample(noiseMap, worldX,worldY,mapSize,mapHeight); 
            }
        }
        return globalNoiseMap;
    }
    private static float BilinearSample(float[,] map, float x, float y, int width, int height)
    {
        int x0 = Mathf.FloorToInt(x) % width;
        int y0 = Mathf.FloorToInt(y) % height;
        int x1 = (x0 + 1) % width;
        int y1 = (y0 + 1) % height;
    
        if (x0 < 0) x0 += width;
        if (y0 < 0) y0 += height;
        if (x1 < 0) x1 += width;
        if (y1 < 0) y1 += height;
    
        float tx = x - Mathf.Floor(x);
        float ty = y - Mathf.Floor(y);
    
        float top = Mathf.Lerp(map[x0, y0], map[x1, y0], tx);
        float bottom = Mathf.Lerp(map[x0, y1], map[x1, y1], tx);
        return Mathf.Lerp(top, bottom, ty);
    }
}
