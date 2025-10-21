using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using TerrainGeneration;
using UnityEngine;
using TerrainGeneration;

/// <summary>
/// Generates a procedural map using Perlin noise and displays it using different modes.
/// Supports drawing noise maps, colored maps, or mesh representations.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    #region MapGenerationVariables

    /// <summary>
    /// Modes for how the map will be drawn/displayed.
    /// </summary>
    public enum DrawMode
    {
        DrawNoiseMap, // Display the raw noise values as a grayscale image
        DrawColorMap, // Display the noise map colored by terrain types
        DrawMesh, // Display the map as a 3D mesh with textures
    }

    public Noise.NormalizedMode normalizedMode;
    [Header("Map Settings")] public DrawMode drawMode; // Current mode for displaying the map
    public const int MapChunkSize = 241; // Size of each map chunk (for mesh generation)
    [Range(0, 6)] public int levelOfDetailEditorPreview; // Level of detail for mesh generation (0 = highest detail)
    [Header("Noise Settings")] public int seed;
    [Range(2, 100)] public float noiseScale; // Scale of the noise (affects zoom)
    [Range(1, 20)] public int octaves; // Number of noise layers combined
    [Range(0, 1)] public float persistence; // Controls amplitude of octaves (affects roughness)
    public float lacunarity; // Controls frequency of octaves (affects detail)
    public AnimationCurve heightCurve; // Curve to adjust height distribution
    public float heightMultiplier;
    public Vector2 offsets;
    [Header("Other Settings")] public bool autoUpdate; // If true, map auto regenerates when settings change
    public bool generateInfiniteTerrain = false;
    public TerrainType[] TerrainTypes; // Array defining different terrain types by height and color
    [CanBeNull] public static float[,] CurrentHeightMap;
    public MeshRenderer meshRenderer;

    #endregion

    #region ThreadingVariables

    private readonly Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new();
    private readonly Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new();

    #endregion

    #region Constructor and Start methods

    private void Start()
    {
        levelOfDetailEditorPreview = Settings.SettingsManager.CurrentSettings.levelOfDetail;
        Debug.Log("Level of Detail from SettingsManager: " + levelOfDetailEditorPreview);
        drawMode = DrawMode.DrawMesh;
        MapData data = GenerateMapData(Vector2.zero);
        MeshGenerator.GenerateTerrainMesh(data.HeightMap, heightMultiplier, heightCurve, levelOfDetailEditorPreview);
        meshRenderer.material.mainTexture = TextureGenerator.TextureFromColourMap(data.ColorMap,MapChunkSize,MapChunkSize);
            
    }

    #endregion

    #region DrawMapInEditor

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        // Find the DisplayMap component in the scene to show the map
        DisplayMap display = FindAnyObjectByType<DisplayMap>();
        //get the TextureRenderer object which is used to draw the 2d texture for the color map draw option and the noise map draw option
        DisplayMap.GetTextureRenderer();
        // Display the map according to the selected draw mode
        switch (drawMode)
        {
            case DrawMode.DrawNoiseMap:
                if (!DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.EnableTextureRenderer();
                }

                display.DisplayNoiseMap(mapData.HeightMap);
                break;

            case DrawMode.DrawColorMap:
                if (!DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.EnableTextureRenderer();
                }

                display.DisplayColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize);
                break;

            case DrawMode.DrawMesh:
                if (DisplayMap.CheckTextureRendererStatus())
                {
                    DisplayMap.DisableTextureRenderer();
                }

                // Generate mesh from noise map and texture from color map
                display.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, heightMultiplier, heightCurve,
                        levelOfDetailEditorPreview),
                    TextureGenerator.TextureFromColourMap(mapData.ColorMap, MapChunkSize, MapChunkSize)
                );
                break;
            default:
                drawMode = DrawMode.DrawNoiseMap;
                break;
        }
    }

    #endregion

    #region GenerateMapData

    /// <summary>
    /// Generates the map data (noise map) and displays it based on the selected draw mode.
    /// </summary>
    public MapData GenerateMapData(Vector2 center)
    {
        // Generate the noise map based on the current parameters
        Debug.Log("Generating noisemap with the center: " + center);
        center.x *= 10000;
        center.y *= 10000;
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize, seed, noiseScale, octaves, persistence,
            lacunarity, center + offsets, normalizedMode);

        // Generate a color map by assigning colors to each noise value based on terrain height
        Color[] colorMap = GenerateColorMap(noiseMap, TerrainTypes);
        return new MapData(noiseMap, colorMap);
    }

    #endregion

    #region GenerateColorMap

    /// <summary>
    /// Converts a noise map into a color map by mapping noise values to terrain types.
    /// </summary>
    /// <param name="noiseMap">2D array of noise values between 0 and 1.</param>
    /// <param name="terrainTypes">Array of terrain types with associated heights and colors.</param>
    /// <returns>1D array of colors representing the map's color data.</returns>
    public static Color[] GenerateColorMap(float[,] noiseMap, TerrainType[] terrainTypes)
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

    #endregion

    #region MapThreading

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(center, callback); };
        new Thread(threadStart).Start();
    }

    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        if (mapData.HeightMap == null)
        {
            Debug.LogError("MapData.HeightMap is null");
            return;
        }

        CurrentHeightMap = mapData.HeightMap;
        lock (_mapDataThreadInfoQueue)
        {
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapdata, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapdata, lod, callback); };
        new Thread(threadStart).Start();
    }

    private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, heightMultiplier, heightCurve, lod);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        Dequeue<MapData>(_mapDataThreadInfoQueue);
        Dequeue<MeshData>(_meshDataThreadInfoQueue);

    }

    private void Dequeue<T>(Queue<MapThreadInfo<T>> queue)
    {
        if (queue.Count ! < 0)
        {
            return;
        }

        int itemsToProcess = queue.Count;
        for (int i = 0; i < itemsToProcess; i++)
        {
            MapThreadInfo<T> threadInfo = queue.Dequeue();
            threadInfo.Callback(threadInfo.Parameter);
        }
    }

    #endregion

    #region ValidateLacunarity

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

    #endregion

    #region JsonWrapperClasses

    [System.Serializable]
    public class SerializableAnimationCurve
    {

        public SerializableKeyframe[] keys;
        public WrapMode preWrapMode;
        public WrapMode postWrapMode;

        public SerializableAnimationCurve(AnimationCurve curve)
        {
            keys = new SerializableKeyframe[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                keys[i] = new SerializableKeyframe(curve.keys[i]);
            }

            preWrapMode = curve.preWrapMode;
            postWrapMode = curve.postWrapMode;
        }

        public AnimationCurve ToAnimationCurve()
        {
            if (keys == null || keys.Length == 0)
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

        public SerializableKeyframe()
        {
        }

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

    #endregion

    #region Structs

    /// <summary>
    /// Struct representing a terrain type with a name, color, and height threshold.
    /// </summary>
    [System.Serializable]
    public struct TerrainType
    {
        public string Name; // Name of the terrain type (e.g., Water, Sand, Grass)
        public Color Color; // Color used to represent this terrain on the map
        public float Height; // Maximum height value that maps to this terrain type
    }

    private struct MapThreadInfo<T>
    {
        public readonly Action<T> Callback;
        public readonly T Parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.Callback = callback;
            this.Parameter = parameter;
        }
    }

    public struct MapData
    {
        public readonly float[,] HeightMap;
        public readonly Color[] ColorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.HeightMap = heightMap;
            this.ColorMap = colorMap;
        }
    }

    #endregion
}