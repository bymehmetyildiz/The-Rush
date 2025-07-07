using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customizable : MonoBehaviour
{
    public List<GameObject> childs = new List<GameObject>();
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

    
    void Update()
    {
        
    }
}
