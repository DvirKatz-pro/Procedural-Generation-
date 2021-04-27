using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    Text text;
    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = "Score: " + score;
    }
    public void updateScore(int amount)
    {
        score += amount;
        text.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
