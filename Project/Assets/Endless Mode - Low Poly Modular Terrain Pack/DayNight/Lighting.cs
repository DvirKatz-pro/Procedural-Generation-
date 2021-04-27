using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Lighting",menuName = "Scriptables/DayNight",order =1)]
public class Lighting : ScriptableObject
{
    public Gradient ambiant;
    public Gradient directional;
    public Gradient fog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
