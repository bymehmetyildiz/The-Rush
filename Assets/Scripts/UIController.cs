using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

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
    public int coinAmount;
    private Character character;
    public bool canSpawnCoin = false;


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
        GameObject newCoin = Instantiate(coinPrefab, transform); // 'transform' here is the UIController's RectTransform, assuming UIController is on the Canvas.
                                                                 // Make sure coinPrefab is a UI element with a RectTransform.

        // Get the RectTransform of the newly spawned coin
        RectTransform coinRectTransform = newCoin.GetComponent<RectTransform>();

        // 1. Calculate the start position (character's world position converted to canvas position)
        Vector3 characterWorldPos = character.transform.position;
        Vector2 startCanvasPos;

        // Get the Canvas component this UIController is on (or find it if it's elsewhere)
        Canvas canvas = GetComponentInParent<Canvas>(); // Assuming UIController is a child of the Canvas or directly on it.
        if (canvas == null)
        {
            Debug.LogError("UIController: No Canvas found as parent or on the same GameObject!");
            yield break; // Exit if no canvas is found
        }

        // Convert character's world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(characterWorldPos);

        // Convert screen position to canvas position
        // RectTransformUtility.ScreenPointToLocalPointInRectangle is the correct way for this.
        // It works for different Canvas Render Modes (Screen Space - Overlay, Screen Space - Camera, World Space).
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, null, out startCanvasPos))
        {
            coinRectTransform.anchoredPosition = startCanvasPos;
        }
        else
        {
            Debug.LogError("Failed to convert character world position to canvas position.");
            // Fallback or handle error if conversion fails
            yield break;
        }

        // 2. Define the end position (the coinText's anchored position)
        // Make sure coinText's parent has a RectTransform and is within the same canvas hierarchy.
        RectTransform coinTextRectTransform = coinText.GetComponent<RectTransform>();
        Vector2 endCanvasPos = coinTextRectTransform.anchoredPosition; // The coinText's anchored position is the target

        float duration = 0.75f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Interpolate between the fixed startCanvasPos and endCanvasPos
            coinRectTransform.anchoredPosition = Vector2.Lerp(startCanvasPos, endCanvasPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches the exact end position
        coinRectTransform.anchoredPosition = endCanvasPos;

        coinAmount++;
        coinText.text = coinAmount.ToString(); // Update the coin text here!
        Destroy(newCoin);
    }

}
