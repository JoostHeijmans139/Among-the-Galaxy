using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise{
    public static float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        //Use arrays instead of lists for performance reasons
        float[,] noiseMap = new float[width, height];
        //Scale value should only be between 0 and 1 not higher and not lower
        if (scale >= 1 || scale <= 0)
        {
            Debug.LogError("Scale must be smaller then 1 and larger then 0 it now is :" + scale);
            return noiseMap;
        }


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;
                float noise = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = noise;
                Debug.Log(noise);
            }
        }
        return noiseMap;
    }
}

