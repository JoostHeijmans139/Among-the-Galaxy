using UnityEngine;

public class DisplayMap : MonoBehaviour
{
    //Define a renderer to display the texture
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    /// <summary>
    /// This function generates and applies a 2d texture based on the provided noise map to visualize the noise values.
    /// </summary>
    /// <param name="noiseMap">a 2d float array of normalized noise values</param>
    public void DisplayNoiseMap(float[,] noiseMap)
    {
        Texture2D texture = TextureGenerator.TextureFromNoiseMap(noiseMap);
        SetFilterOptions(texture);
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width,1,texture.height);
    }

    public void DisplayColorMap(Color[] colorMap, int mapWidth, int mapHeight)
    {
        Texture2D texture = TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight);
        SetFilterOptions(texture);
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width,1,texture.height);
    }
    //NOT SURE IF THIS WORKS DO PROPER CHECK
    public void DrawMesh(MeshData meshData,Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(texture.width,1,texture.height);
    }

    private void SetFilterOptions(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
    }
}
