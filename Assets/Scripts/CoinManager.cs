using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    

    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Rotate(0, 0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Character>() != null)
        {      
            UIController.instance.canSpawnCoin = true;
            Destroy(gameObject);
        }
    }
}
