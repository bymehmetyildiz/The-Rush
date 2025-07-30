using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_SlotManager : MonoBehaviour
{
    public CustomizableType customizableType;
    [SerializeField] private bool isUnlocked;    
    [SerializeField] private Image lockImage;
    [SerializeField] private int cost;
    [SerializeField] private TMP_Text costText;

    public Customizable relational;    
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
            if (UIController.instance.currentSlot != this)
            {
                relational.Activate(index, customizableType);
                UIController.instance.currentSlot = this;
                UIController.instance.frame.gameObject.SetActive(true);
                UIController.instance.frame.SetParent(this.transform);
                UIController.instance.frame.anchoredPosition = Vector2.zero;                
            }
            else if (UIController.instance.currentSlot == this)
            {
                relational.Deactivate(index);
                UIController.instance.currentSlot = null;
                UIController.instance.frame.gameObject.SetActive(false);                
            }
        }
        else
        { 
            if (UIController.instance.coinAmount >= cost)
            {
                UIController.instance.coinAmount -= cost;
                UIController.instance.coinText.text = FormatNumber(UIController.instance.coinAmount);
                isUnlocked = true;                  
                lockImage.gameObject.SetActive(false);
                costText.gameObject.SetActive(false);               
                relational.Activate(index, customizableType);
                UIController.instance.currentSlot = this;
                UIController.instance.frame.gameObject.SetActive(true);
                UIController.instance.frame.SetParent(this.transform);
                UIController.instance.frame.anchoredPosition = Vector2.zero;
            }
            else
            {
                Debug.Log("Not enough coins to unlock this slot.");
            }
        }
        
    }

    public bool IsEquipped()
    {
        if(relational.transform.GetChild(index).gameObject.activeSelf)
        {
            return true;
        }
        return false;
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
