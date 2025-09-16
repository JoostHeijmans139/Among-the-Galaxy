using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Noise.GenerateNoiseMap(10,10,0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
