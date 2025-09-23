
using UnityEngine;

public class MapGenerator:MonoBehaviour
{
    public enum DrawMode
    {
        DrawNoiseMap,
        DrawColorMap,
        DrawMesh,
    }
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    [Range(2,100)]
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
    public bool autoUpdate;
    public TerrainType[] TerrainTypes;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapHeight,noiseScale,octaves,persistence,lacunarity);
        DisplayMap display = FindObjectOfType<DisplayMap>();
        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < noiseMap.GetLength(0); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(1); x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < TerrainTypes.Length; i++)
                {
                    if (currentHeight <= TerrainTypes[i].Height)
                    {
                        colorMap[y * mapWidth + x] = TerrainTypes[i].Color;
                        break;
                    }
                }
                    
            }
        }
        switch (drawMode)
        {
            case DrawMode.DrawNoiseMap:
                display.DisplayNoiseMap(noiseMap);
                break;
            case DrawMode.DrawColorMap:

                display.DisplayColorMap(colorMap,mapWidth, mapHeight);
                break;
            case DrawMode.DrawMesh:
                display.DrawMesh(MeshGenerator.GenerateTerainMesh(noiseMap), TextureGenerator.TextureFromColourMap(colorMap,mapWidth,mapHeight));
                break;
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
    [System.Serializable]

    public struct TerrainType
    {
        public string Name;
        public Color Color;
        public float Height;
    }
}
