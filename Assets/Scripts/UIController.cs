using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class UIController : MonoBehaviour
{
    //Instance
    public static UIController instance;

    [Header("Buttons")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pausePanel;

    [Header("Score")]
    public TMP_Text scoreText;
    public int score;

    [Header("Coin")]
    [SerializeField] private GameObject coinPrefab;     
    [SerializeField] private GameObject coinEndPoint;
    [SerializeField] private Vector2 coinSpawnPoint;
    [SerializeField] private float offset;
    public TMP_Text coinText;    
    public int coinAmount;
    private Character character;
    public bool canSpawnCoin = false;

    [Header("Customize Panel")]
    [SerializeField] private RectTransform[] categories;
    [SerializeField] private RectTransform current;
    [SerializeField] private TMP_Text headerText;
    private int currentIndex = 0;
    private bool isSnapping;
    [SerializeField] private RectTransform customizePanel;

    [Header("Frame")]
    public UI_SlotManager currentSlot;
    public RectTransform frame;

   
    

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

        coinAmount = 100000;
        coinText.text = FormatNumber(coinAmount);

        for (int i = 0; i < categories.Length; i++)
        {
            categories[i].anchoredPosition = new Vector2(600, 445);
        }
        categories[0].anchoredPosition = new Vector2(0, 445);
        current = categories[0];
        headerText.text = categories[0].gameObject.name;   

        frame.gameObject.SetActive(false);

        customizePanel.anchoredPosition = new Vector2(-1000, 0);
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

    public void SlidePanels(bool toRight)
    {
        if(toRight)
        {
            if (currentIndex < categories.Length - 1 && !isSnapping)
            {
                isSnapping = true;
                categories[currentIndex].DOAnchorPos(new Vector2(-600, 445), 0.25f);
                currentIndex++;
                headerText.text = categories[currentIndex].gameObject.name;
                categories[currentIndex]
                    .DOAnchorPos(new Vector2(0, 445), 0.5f)
                    .SetEase(Ease.OutBack)
                   .OnComplete(() =>
                   {
                       current = categories[currentIndex];
                       isSnapping = false;

                       UI_SlotManager[] slots = current.GetComponentsInChildren<UI_SlotManager>();

                       foreach (UI_SlotManager slot in slots)
                       {
                           if (slot.IsEquipped())
                           {
                               currentSlot = slot;
                               frame.gameObject.SetActive(true);
                               frame.SetParent(currentSlot.transform);
                               frame.anchoredPosition = Vector2.zero;
                               break;
                           }
                           else
                           {
                               currentSlot = null;
                               frame.gameObject.SetActive(false);
                           }
                       }
                   });
            }
            else
                return;
        }
        else
        {
            if (currentIndex > 0 && !isSnapping)
            {
                isSnapping = true;
                categories[currentIndex].DOAnchorPos(new Vector2(600, 445), 0.25f);
                currentIndex--;
                headerText.text = categories[currentIndex].gameObject.name;
                categories[currentIndex]
                    .DOAnchorPos(new Vector2(0, 445), 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        current = categories[currentIndex];
                        isSnapping = false;

                        UI_SlotManager[] slots = current.GetComponentsInChildren<UI_SlotManager>();

                        foreach (UI_SlotManager slot in slots)
                        {
                            if (slot.IsEquipped())
                            {
                                currentSlot = slot;
                                frame.gameObject.SetActive(true);
                                frame.SetParent(currentSlot.transform);
                                frame.anchoredPosition = Vector2.zero;
                                break;                                
                            }
                            else
                            {
                                currentSlot = null;
                                frame.gameObject.SetActive(false);
                            }

                        }

                    });
            }
            else
                return;          
        }
    }

    public void OpenCustomizePanel()
    {
        if(customizePanel.anchoredPosition.x != -200)
            customizePanel.DOAnchorPos(new Vector2(-200, 0), 0.5f).SetEase(Ease.OutBack);
        else if (customizePanel.anchoredPosition.x != -1000)
            customizePanel.DOAnchorPos(new Vector2(-1000, 0), 0.5f).SetEase(Ease.InBack);
    }

    string FormatNumber(long number)
    {
        if (number >= 1_000_000_000_000_000)
            return (number / 1_000_000_000_000_000f).ToString("0.#") + "Q";
        else if (number >= 1_000_000_000_000)
            return (number / 1_000_000_000_000f).ToString("0.#") + "T";
        else if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString("0.#") + "B";
        else if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "K";
        else
            return number.ToString();
    }
}
