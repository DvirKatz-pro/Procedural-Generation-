using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for generating 2 noise maps as quads - Dvir
public class NoiseQuad : MonoBehaviour
{
    [SerializeField] private List<GameObject> terrainCPT;
    private bool spawn = true;
    //number of pixels
    [SerializeField] private int rowLen = 256;
    [SerializeField] private int colLen = 256;

    [SerializeField] private float scale = 20;
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetY = 0;

    public int octaves;
    public float persistance;
    public float lacunarity;
    public bool fractal = false;
   
    [SerializeField] private GameObject noiseObject;
    NoiseGenerator noise;
    Renderer rd;
    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Renderer>();
        noise = noiseObject.GetComponent<NoiseGenerator>();
        
    }

    private void Update()
    {
        //generate a fractal noise map or a regular one
        if (fractal)
        {
            rd.material.mainTexture = generateFractalTexture();
        }
        else
        {
            rd.material.mainTexture = generateTexture();
        }
    }
    //based on a noise map generate a texture for a regular noise map
    Texture2D generateTexture()
    {
        Texture2D texture = new Texture2D(rowLen, colLen);
        float[,] noiseMap = noise.generateNoiseMap(rowLen, colLen, scale, new Vector2(offsetX, offsetY));
        for (int i = 0; i < rowLen; i++)
        {
            for (int j = 0; j < colLen; j++)
            {
                float noiseValue = noiseMap[i, j];
                Color color = new Color(noiseValue, noiseValue, noiseValue);
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        return texture;
    }
    //based on a noise map generate a texture for a fractal noise map
    Texture2D generateFractalTexture()
    {
        Texture2D texture = new Texture2D(rowLen, colLen);
      
        float[,] noiseValue = noise.generateFractalNoiseMap(rowLen, colLen, scale, octaves, persistance, lacunarity,new Vector2(offsetX,offsetY));
        for (int i = 0; i < rowLen; i++)
        {
            for (int j = 0; j < colLen; j++)
            {
               
                Color color = new Color(noiseValue[i, j], noiseValue[i, j], noiseValue[i, j]);
                
                
                texture.SetPixel(i, j, color);
            }
        }
       
        texture.Apply();
        return texture;
    }
}
