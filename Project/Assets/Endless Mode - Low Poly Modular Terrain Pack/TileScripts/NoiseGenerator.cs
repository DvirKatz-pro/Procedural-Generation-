using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for generating noise maps- Dvir
public class NoiseGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //generate a noise value based on given parameters
    public float[,] generateNoiseMap(int x, int y,float scale,Vector2 offset)
    {
        float[,] noiseMap = new float[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float sampleX = (float)i / (float)x * scale + (offset.x);
                float sampleZ = (float)j / (float)y * scale + (offset.y);
                sampleX += 0.0001f;
                sampleZ += 0.0001f;
                float noise = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[i, j] = noise;
            }
        }

       
        
       // noise = Mathf.Abs(noise);
        


        return noiseMap;

    }
    //generate a fractal noise map and return it
    public float[,] generateFractalNoiseMap(int x, int y, float scale, int octaves, float persistance, float lacunarity,Vector2 offset)
    {
        float[,] noiseMap = new float[x,y];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float amplitude = 1;
                float frequancy = 1;
                float noiseHeight = 0;
                for (int k = 0; k < octaves; k++)
                {
                    float sampleX = i / scale * frequancy + (offset.x/scale * frequancy);
                    float sampleY = j / scale * frequancy + (offset.y/scale * frequancy);

                    float noise = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noise = Mathf.Abs(noise);
                

                    noiseHeight += noise * amplitude;
                    amplitude *= persistance;
                    frequancy *= lacunarity;

                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                noiseMap[i, j] = noiseHeight;
                if (noiseMap[i, j] >= 0.0085)
                {
                    noiseMap[i, j] += 0.1f;
                }

            }
        }
       
      
        return noiseMap;
    }
    
}
