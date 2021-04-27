using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script to check for seed input - Dvir
public class SeedInput : MonoBehaviour
{
    TerrainGeneration terrain;
   
    // Start is called before the first frame update
    void Start()
    {
        //text = GetComponent<InputField>().text;
        terrain = GameObject.Find("TerrainGenerator").GetComponent<TerrainGeneration>();
    }

    // Update is called once per frame
    void Update()
    {
        string inp = GetComponent<InputField>().text;
        string[] se = inp.Split(' ');
        if (inp.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            //format input field and see if its valid input, if it is, set it as world seed
            List<float> nums = new List<float>();
            foreach (string s in se)
            {
                nums.Add(float.Parse(s));
            }
            if (nums.Count == 10)
            {
                terrain.setWorldSeed(nums);
            }
        }
    }
}
