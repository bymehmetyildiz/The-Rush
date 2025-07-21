using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject pauseButton;
    public TMP_Text scoreText;
    public int score;

    void Start()
    {
        pauseButton.SetActive(false);
        score = 0;
        scoreText.text = score.ToString() + " m";
    }

    
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (startButton.activeSelf == true)
        {
            startButton.SetActive(false);
            pauseButton.SetActive(true);
        }        
    }


}
