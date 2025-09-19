using UnityEngine;

public static class Noise{
    /// <summary>
    /// This function generates a 2d float array of noise values which represent a noiseMap which is used in the terrain generation
    /// </summary>
    /// <param name="width">The actual map width</param>
    /// <param name="height">The actual map height</param>
    /// <param name="scale">Decides the zoom level of the noise higher means smoother terrain smaller means more details and bumpy terrain</param>
    /// <param name="octaves">Decides the amount of noise differences</param>
    /// <param name="persistence">Decides how much the amplitude changes with each octave</param>
    /// <param name="lacunarity">Decides how much the frequency changes with each</param>
    /// <returns>A 2d float array</returns>
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity)
    {
        //Use arrays instead of lists for performance reasons
        float[,] noiseMap = new float[width, height];
        //Scale value should only be between 0 and 1 not higher and not lower
        if (scale >= 1 || scale <= 0)
        {
            Debug.LogError("Scale must be smaller then 1 and larger then 0 it now is :" + scale);
            return noiseMap;
        }
        //its backwards set so any value found is either less or more 
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        // Generate a 2D noise map using layered (octave) Perlin noise for procedural terrain.
        // For each (x, y) coordinate, sum multiple octaves of Perlin noise with varying frequency and amplitude.
        // Track the minimum and maximum noise heights for later normalization.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //amplitude hase influence on the actual height of the terrain
                float amplitude = 1;
                //frequency hase influence on the small details on the terrain higher means more lower means less
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }
        //normalize noise values to make sure they are in between 0 and 1
        noiseMap = normalizeNoiseMap(noiseMap, minNoiseHeight, maxNoiseHeight,width,height);
        return noiseMap;
    }

    /// <summary>
    /// This is a function to normalize the values inside the given noisemap to make sure they are between 0 and 1
    /// </summary>
    /// <param name="noiseMap">A 2d float array of unnormalized values</param>
    /// <param name="minNoiseHeight">the minimum noise height value being passed through from noise generation</param>
    /// <param name="maxNoiseHeight">the maximum noise height value being passed through from noise generation</param>
    /// <param name="mapWidth">The map width for the length of the inner loop</param>
    /// <param name="mapHeight">The map height for the length of the outer loop</param>
    /// <returns>a 2d float array of normalized values</returns>
    private static float[,] normalizeNoiseMap(float[,] noiseMap, float minNoiseHeight, float maxNoiseHeight,int mapWidth,int mapHeight)
    {
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

