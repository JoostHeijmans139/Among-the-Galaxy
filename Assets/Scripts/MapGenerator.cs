using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    void Start()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapHeight,noiseScale);
    }
    
}
