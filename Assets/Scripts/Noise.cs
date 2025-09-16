using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise{
    //Scale value should only be between 0 and 1 not higher and not lower
    public static List<List<float>> GenerateNoiseMap(int width, int height, float scale)
    {
        List<List<float>> noiseMap = new();
        if (scale >= 1 || scale <= 0)
        {
            Debug.LogError("Scale must be smaller then 1 and larger then 0 it now is :" + scale);
        }


        for (int y = 0; y < height; y++)
        {
            List<float> row = new();
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;
                float noise = Mathf.PerlinNoise(sampleX, sampleY);
                row.Add(noise);
                Debug.Log(noise);
            }
            noiseMap.Add(row);
        }
        foreach (var row in noiseMap)
        {
            foreach (var noise in row)
            {
                Debug.Log(noise);
            }   
        }
        return noiseMap;
    }
}

