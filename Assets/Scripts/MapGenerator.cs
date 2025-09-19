
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        DrawNoiseMap,
        DrawColorMap,
    }
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
    public bool autoUpdate;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapHeight,noiseScale,octaves,persistence,lacunarity);
        DisplayMap display = FindObjectOfType<DisplayMap>();
        if (drawMode == DrawMode.DrawNoiseMap)
        {
            display.DisplayNoiseMap(noiseMap);
        }
        
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = mapWidth;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
        
    }

    public struct TerrainType
    {
        public string Name;
        public Color Color;
        public float Height;
    }
}
