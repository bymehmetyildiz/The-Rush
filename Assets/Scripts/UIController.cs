using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    //Instance
    public static UIController instance;

    //Buttons
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pausePanel;

    //Score
    public TMP_Text scoreText;
    public int score;

    //Coin
    [SerializeField] private GameObject coinPrefab;     
    [SerializeField] private TMP_Text coinText;    
    [SerializeField] private float offset;    
    public int coinAmount;
    private Character character;
    public bool canSpawnCoin = false;
    [SerializeField] private Vector2 coinSpawnPoint;
    [SerializeField] private GameObject coinEndPoint;


    private void Awake()
    {
        if(instance == null)        
            instance = this;        
        else        
            Destroy(gameObject);        
    }


    void Start()
    {
        pauseButton.SetActive(false);
        pausePanel.SetActive(false);
        score = 0;
        scoreText.text = score.ToString() + " m";
        character = FindObjectOfType<Character>();   
        canSpawnCoin = false;

        coinText.text = coinAmount.ToString();
    }

    
    void Update()
    {
        if (canSpawnCoin == true)
        {
            canSpawnCoin = false;
            StartCoroutine(SpawnCoin());
        }
    }

    public void StartGame()
    {
        if (startButton.activeSelf == true)
        {
            startButton.SetActive(false);
            pauseButton.SetActive(true);
        }        
    }

    public void PauseGame()
    {
        if (pausePanel.activeSelf == false)
        {
            pausePanel.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            pausePanel.SetActive(false);
            pauseButton.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    private IEnumerator SpawnCoin()
    {
        GameObject newCoin = Instantiate(coinPrefab, transform);
        coinSpawnPoint = Camera.main.WorldToScreenPoint(character.transform.position + Vector3.up * offset);
        newCoin.transform.position = coinSpawnPoint;
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            newCoin.transform.position = Vector2.Lerp(newCoin.transform.position, coinEndPoint.transform.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        newCoin.transform.position = coinEndPoint.transform.position;
        coinAmount++;
        coinText.text = coinAmount.ToString();        
        Destroy(newCoin);
    }

}
