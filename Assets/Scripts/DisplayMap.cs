using UnityEngine;

public class DisplayMap : MonoBehaviour
{
    //Define a renderer to display the texture
    public Renderer textureRenderer;
    
    /// <summary>
    /// This function generates and applies a 2d texture based on the provided noise map to visualize the noise values.
    /// </summary>
    /// <param name="noiseMap">a 2d float array of normalized noise values</param>
    public void DrawNoiseMap(float[,] noiseMap)
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
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width,1,height);
    }
}
