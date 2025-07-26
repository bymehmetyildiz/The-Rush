using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SlotManager : MonoBehaviour
{
    [SerializeField] private bool isUnlocked;
    [SerializeField] private Image lockImage;
    [SerializeField] private int cost;
    [SerializeField] private TMP_Text costText;

    [SerializeField] private Customizable relational;
    [SerializeField] private int index;


    void Start()
    {
        costText = GetComponentInChildren<TMP_Text>();
        costText.text = FormatNumber(cost);


        if (isUnlocked)
        {
            lockImage.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
        }

    }

    public void UnlockSlot()
    {
        if (isUnlocked)
        {
            relational.Activate(index);
        }
        else
        {
            if (UIController.instance.coinAmount >= cost)
            {
                UIController.instance.coinAmount -= cost;
                isUnlocked = true;
                lockImage.gameObject.SetActive(false);
                costText.gameObject.SetActive(false);
                relational.Activate(index); // Activate the first child of the customizable object
            }
            else
            {
                Debug.Log("Not enough coins to unlock this slot.");
            }
        }
        
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
