using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customizable : MonoBehaviour
{
    public List<GameObject> childs = new List<GameObject>();
    public CustomizableType customizableType;
    public bool isOptional;    
    public GameObject objToHide;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;                    
            childs.Add(child);
            child.SetActive(false);
        }
        if (!isOptional)
        {
            childs[0].SetActive(true);
            childs.RemoveAt(0);
        }
    }

    public void Activate(int index, CustomizableType _customizableType)
    {
       if(objToHide != null)
            objToHide.SetActive(false);

        for (int i = 0; i < childs.Count; i++)
        {
            if(_customizableType == customizableType)
                childs[i].SetActive(false);
        }

        childs[index].SetActive(true);
    }

    public void Deactivate(int index)
    {
        if(childs[index] != null)       
            childs[index].SetActive(false);

        if (objToHide != null)
            objToHide.SetActive(true);
    }
}

public enum CustomizableType
{
    Beards,
    Bracelets,
    Eyebrows,
    Gloves,
    Hair,
    Masks,
    Mustaches,
    Pants,
    Scarfs,
    Shirts,
    Shoes
}
