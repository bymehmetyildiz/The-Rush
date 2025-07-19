using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private GameObject currentBlock;
    [SerializeField] private GameObject firstBlock;
    [SerializeField] private GameObject secondBlock;
    [SerializeField] private GameObject thirdBlock;
    [SerializeField] private GameObject fourthBlock;
    [SerializeField] private Character character;
    


    void Start()
    {
        character = FindObjectOfType<Character>();
        firstBlock = Instantiate(blocks[0], new Vector3(0, 0, 0), Quaternion.identity);
        secondBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 12), Quaternion.identity);
        thirdBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 24), Quaternion.identity);
        fourthBlock = Instantiate(blocks[Random.Range(0, 19)], new Vector3(0, 0, 0 + 36), Quaternion.identity);
        currentBlock = secondBlock;
    }


    void Update()
    {
       if(character.transform.position.z > fourthBlock.transform.position.z - 10)
       {
           if(character.transform.position.z > currentBlock.transform.position.z)
           {
              
                firstBlock = secondBlock;
               secondBlock = thirdBlock;
               thirdBlock = fourthBlock;
               fourthBlock = Instantiate(blocks[Random.Range(0, 19)], new Vector3(0, 0, fourthBlock.transform.position.z + 12), Quaternion.identity);
            }
        }

    }

   

   

}
