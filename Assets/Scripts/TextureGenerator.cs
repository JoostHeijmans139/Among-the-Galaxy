

using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //generate a color based on the noise value between black and white and add that to a colour map for the 2d texture
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromColourMap(Color[] colourMap,int mapWidth,int mapHeight)
    {

        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
}
