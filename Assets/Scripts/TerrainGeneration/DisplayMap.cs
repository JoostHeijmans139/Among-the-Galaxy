using TerrainGeneration;
using UnityEngine;

/// <summary>
/// Responsible for displaying generated maps by applying textures and meshes to renderers in the scene.
/// Supports displaying noise maps as textures, color maps as textures, and terrain meshes with textures.
/// </summary>
public class DisplayMap : MonoBehaviour
{
    [Header("Rendering Components")] 
    // Renderer used to display 2D textures (noise or color maps)
    private static Renderer _textureRendererStatic;
    public MeshFilter meshFilter;       // MeshFilter component for displaying mesh geometry
    public MeshRenderer meshRenderer;   // MeshRenderer component for applying textures to meshes
    

    /// <summary>
    /// Generates and applies a 2D grayscale texture based on the provided noise map.
    /// </summary>
    /// <param name="noiseMap">2D array of normalized noise values (0 to 1)</param>
    public void DisplayNoiseMap(float[,] noiseMap)
    {
        // Create a texture from the noise map
        Texture2D texture = TextureGenerator.TextureFromNoiseMap(noiseMap);

        // Configure texture filter and wrap mode
        SetFilterOptions(texture);

        // Apply the generated texture to the renderer's material
        _textureRendererStatic.sharedMaterial.mainTexture = texture;

        // Scale the renderer's transform to match texture size (width x height)
        _textureRendererStatic.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    /// <summary>
    /// Generates and applies a color texture map based on the given color data.
    /// </summary>
    /// <param name="colorMap">1D array of colors representing the map pixels</param>
    /// <param name="mapWidth">Width of the color map</param>
    /// <param name="mapHeight">Height of the color map</param>
    public void DisplayColorMap(Color[] colorMap, int mapWidth, int mapHeight)
    {
        // Create a texture from the provided color map
        Texture2D texture = TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight);

        // Set texture filtering and wrapping options
        SetFilterOptions(texture);

        // Assign the texture to the renderer's material
        _textureRendererStatic.sharedMaterial.mainTexture = texture;

        // Scale renderer to match the texture's dimensions
        _textureRendererStatic.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    /// <summary>
    /// Draws a mesh generated from terrain data and applies a texture to it.
    /// Also applies the same texture to the 2D texture renderer (optional).
    /// </summary>
    /// <param name="meshData">MeshData object containing vertices, triangles, and UVs</param>
    /// <param name="texture">Texture to apply to the mesh</param>
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        // Create and assign the mesh to the MeshFilter for rendering
        meshFilter.sharedMesh = meshData.CreateMesh();

        // Assign the texture to the MeshRenderer's material
        meshRenderer.sharedMaterial.mainTexture = texture;

        // Scale the mesh renderer to fit the texture's width and height
        meshRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);

        // Also assign the texture to the textureRenderer and scale it to zero height (effectively hiding it)
        // This may be to keep the texture renderer synchronized or for visual fallback
        // textureRenderer.sharedMaterial.mainTexture = texture;
        // textureRenderer.transform.localScale = new Vector3(texture.width, 0, texture.height);
    }

    /// <summary>
    /// Configures texture filtering and wrapping settings to ensure pixel-perfect rendering without tiling.
    /// </summary>
    /// <param name="texture">Texture2D to configure</param>
    private void SetFilterOptions(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;           // Disable texture smoothing for crisp pixels
        texture.wrapMode = TextureWrapMode.Clamp;        // Prevent texture repetition beyond edges
    }

    /// <summary>
    /// Disables the static texture renderer, making it invisible in the scene.
    /// </summary>
    public static void DisableTextureRenderer()
    {
        _textureRendererStatic.enabled = false;
    }

    /// <summary>
    /// Enables the static texture renderer, making it visible in the scene.
    /// </summary>
    public static void EnableTextureRenderer()
    {
        _textureRendererStatic.enabled = true;
    }

    /// <summary>
    /// Checks whether the static texture renderer is currently enabled (visible).
    /// </summary>
    /// <returns>True if the texture renderer is enabled; otherwise, false.</returns>
    public static bool CheckTextureRendererStatus()
    {
        return _textureRendererStatic.enabled;
    }

    /// <summary>
    /// Finds and assigns the static texture renderer by searching for a GameObject with the "Floor" tag.
    /// Logs errors if the GameObject or its Renderer component is not found.
    /// </summary>
    public static void GetTextureRenderer()
    {
        GameObject textureRendererStatic = GameObject.FindWithTag("Floor");
        if (!textureRendererStatic)
        {
            Debug.LogError("No game object found with tag Floor");
        }
        _textureRendererStatic = textureRendererStatic.GetComponent<Renderer>();
        if (_textureRendererStatic == null)
        {
            Debug.Log("Render not found on game object that hase the Floor tag");
        }
    }

}
