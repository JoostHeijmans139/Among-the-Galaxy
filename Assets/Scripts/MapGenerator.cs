using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
    private const int MapChunkSize = 241; // Size of each map chunk (for mesh generation)
    [Range(0,6)]
    public int levelOfDetail;      // Level of detail for mesh generation (0 = highest detail)
    [Header("Noise Settings")]
    public int seed;
    [Range(2, 100)]
    public float noiseScale;         // Scale of the noise (affects zoom)
    [Range(1, 20)]
    public int octaves;              // Number of noise layers combined
    [Range(0, 1)]
    public float persistence;        // Controls amplitude of octaves (affects roughness)
    public float lacunarity;         // Controls frequency of octaves (affects detail)
    public AnimationCurve heightCurve; // Curve to adjust height distribution
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
        //get the TextureRenderer object which is used to draw the 2d texture for the color map draw option and the noise map draw option
        DisplayMap.GetTextureRenderer();
        // Generate the noise map based on the current parameters
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize,seed, noiseScale, octaves, persistence, lacunarity,offsets);

        // Find the DisplayMap component in the scene to show the map
        DisplayMap display = FindObjectOfType<DisplayMap>();

        // Generate a color map by assigning colors to each noise value based on terrain height
        Color[] colorMap = GenerateColorMap(noiseMap, TerrainTypes);

        // Display the map according to the selected draw mode
        switch (drawMode)
        {
            case DrawMode.DrawNoiseMap:
                if (!DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.EnableTextureRenderer();
                }
                display.DisplayNoiseMap(noiseMap);
                break;

            case DrawMode.DrawColorMap:
                if (!DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.EnableTextureRenderer();
                }
                display.DisplayColorMap(colorMap, MapChunkSize, MapChunkSize);
                break;

            case DrawMode.DrawMesh:
                if (DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.DisableTextureRenderer();
                }
                // Generate mesh from noise map and texture from color map
                display.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(noiseMap,heightMultiplier,heightCurve,levelOfDetail),
                    TextureGenerator.TextureFromColourMap(colorMap, MapChunkSize, MapChunkSize)
                );
                break;
            default:
                drawMode = DrawMode.DrawNoiseMap;
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
        if (lacunarity < 1)
        {
            lacunarity = 1; // Lacunarity should be at least 1
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
    [System.Serializable]
    public class SerializableAnimationCurve
    {
        
        public SerializableKeyframe[] keys;
        public WrapMode preWrapMode;
        public WrapMode postWrapMode;
        
        public SerializableAnimationCurve(AnimationCurve curve)
        {
            keys = new SerializableKeyframe[curve.keys.Length];
            for(int i = 0; i < curve.keys.Length; i++)
            {
                keys[i] = new SerializableKeyframe(curve.keys[i]);
            }
            preWrapMode = curve.preWrapMode;
            postWrapMode = curve.postWrapMode;
        }

        public AnimationCurve ToAnimationCurve()
        {
            if(keys == null || keys.Length == 0)
            {
                Debug.LogWarning("No keyframes found in SerializableAnimationCurve. Returning default AnimationCurve.");
                return new AnimationCurve();
            }
            Keyframe[] keyframes = new Keyframe[keys.Length];
            Debug.Log("Converting SerializableAnimationCurve with " + keys.Length + " keyframes to AnimationCurve.");
            for (int i = 0; i < keys.Length; i++)
            {
                keyframes[i] = keys[i].ToKeyframe();
            }
            Debug.Log("Found " + keyframes.Length + " keyframes. after deserialization.");
            var curve = new AnimationCurve(keyframes)
            {
                preWrapMode = preWrapMode,
                postWrapMode = postWrapMode,
            };
            return curve;
        }
    }
    [Serializable]
    public class SerializableKeyframe
    {
        public float time;
        public float value;
        public float inTangent;
        public float outTangent;
        public float inWeight;
        public float outWeight;
        public WeightedMode weightedMode;

        public SerializableKeyframe() { }

        public SerializableKeyframe(Keyframe keyframe)
        {
            time = keyframe.time;
            value = keyframe.value;
            inTangent = keyframe.inTangent;
            outTangent = keyframe.outTangent;
            inWeight = keyframe.inWeight;
            outWeight = keyframe.outWeight;
            weightedMode = keyframe.weightedMode;
        }

        public Keyframe ToKeyframe()
        {
            Keyframe kf = new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
            kf.weightedMode = weightedMode;
            return kf;
        }
    }
}
