using UnityEngine;

/// <summary>
/// Static class responsible for generating Texture2D objects from noise maps or color maps.
/// </summary>
public static class TextureGenerator
{
    /// <summary>
    /// Creates a grayscale Texture2D from a 2D noise map.
    /// Noise values (0 to 1) are converted to colors ranging from black to white.
    /// </summary>
    /// <param name="noiseMap">2D array of normalized noise values (float between 0 and 1).</param>
    /// <returns>A Texture2D object representing the noise map as a grayscale image.</returns>
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        // Create a new texture with the same dimensions as the noise map
        Texture2D texture = new Texture2D(width, height);

        // Initialize an array to hold color data for each pixel
        Color[] colourMap = new Color[width * height];

        // Loop through every pixel position to convert noise values to grayscale colors
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Interpolate between black and white based on noise value at (x, y)
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        // Assign the generated color map to the texture's pixels
        texture.SetPixels(colourMap);

        // Apply changes to update the texture on the GPU
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Creates a Texture2D from a color map array.
    /// </summary>
    /// <param name="colourMap">1D array of colors representing each pixel.</param>
    /// <param name="mapWidth">Width of the texture.</param>
    /// <param name="mapHeight">Height of the texture.</param>
    /// <returns>A Texture2D object constructed from the given color map.</returns>
    public static Texture2D TextureFromColourMap(Color[] colourMap, int mapWidth, int mapHeight)
    {
        // Create a new texture with specified dimensions
        Texture2D texture = new Texture2D(mapWidth, mapHeight);

        // Assign the provided color map to the texture pixels
        texture.SetPixels(colourMap);

        // Apply changes to upload texture data to the GPU
        texture.Apply();

        return texture;
    }
}
