using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private GameObject currentBlock;
    [SerializeField] private GameObject previousBlock;
    [SerializeField] private GameObject nextBlock;
    [SerializeField] private Character character;


    void Start()
    {
        character = FindObjectOfType<Character>();
        previousBlock = Instantiate(blocks[0], new Vector3(0, 0, 0), Quaternion.identity);
        currentBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 12), Quaternion.identity);
        nextBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 24), Quaternion.identity);
    }

    
    void Update()
    {
        if(character.transform.position.z > currentBlock.transform.position.z)
        {
            Destroy(previousBlock, 2);
            previousBlock = currentBlock;
            currentBlock = nextBlock;
            nextBlock = Instantiate(blocks[Random.Range(0, blocks.Length)], new Vector3(0, 0, currentBlock.transform.position.z + 12), Quaternion.identity);
        }
    }
}
