using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise{
    //Scale value should only be between 0 and 1 not higher and not lower
    public static List<List<float>> GenerateNoiseMap(int width, int height, float scale)
    {
        List<List<float>> noiseMap = new();
        if (scale <= 0)
        {
            scale = .0001f;
        }
        else if (scale >= 1)
        {
            Debug.LogError("Scale must be smaller then 1 it now is :" + scale);
        }
        return noiseMap;
    }
}

