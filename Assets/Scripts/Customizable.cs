using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customizable : MonoBehaviour
{
    public List<GameObject> childs = new List<GameObject>();
    public CustomizableType customizableType;
    public bool isOptional;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            childs.Add(child);
            child.SetActive(false); // Deactivate all children initially
        }
        if (!isOptional)
        {
            childs[0].SetActive(true);
        }

    }

    public void Activate(int index, CustomizableType _customizableType)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            if(_customizableType == customizableType)
                childs[i].SetActive(false); // Deactivate all children
        }

        childs[index].SetActive(true); // Activate the specified child
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
