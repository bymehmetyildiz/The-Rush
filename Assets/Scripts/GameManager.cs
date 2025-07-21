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
    private Direction direction = Direction.Forward;

    public enum Direction
    {
        Forward,
        Rigth,
        Left,
    }



    void Start()
    {
        character = FindObjectOfType<Character>();
        firstBlock = Instantiate(blocks[0], new Vector3(0, 0, 0), Quaternion.identity);
        secondBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 12), Quaternion.identity);
        thirdBlock = Instantiate(blocks[0], new Vector3(0, 0, 0 + 24), Quaternion.identity);
        fourthBlock = Instantiate(blocks[Random.Range(0, 3)], new Vector3(0, 0, 0 + 36), Quaternion.identity);
        currentBlock = secondBlock;
    }


    void Update()
    {        
        if(character.canMove == false)        
            return;
        
        if (direction == Direction.Forward)
        {
            if (character.transform.position.z >= currentBlock.transform.position.z + 2)
            {
                Destroy(firstBlock, 3);
                firstBlock = secondBlock;
                secondBlock = thirdBlock;
                thirdBlock = fourthBlock;
                Block block = thirdBlock.GetComponent<Block>();

                if (block.blockType == Block.BlockType.Straight)
                {
                    fourthBlock = Instantiate(blocks[Random.Range(0, blocks.Length)], new Vector3(thirdBlock.transform.position.x, 0, thirdBlock.transform.position.z + 12), Quaternion.identity);
                }
                else if (block.blockType == Block.BlockType.LeftCorner)
                {
                    fourthBlock = Instantiate(blocks[Random.Range(0, blocks.Length-2)], new Vector3(thirdBlock.transform.position.x - 4, 0, thirdBlock.transform.position.z), Quaternion.identity);
                    fourthBlock.transform.rotation = Quaternion.Euler(0, -90, 0);
                    direction = Direction.Left;
                }
                else if (block.blockType == Block.BlockType.RightCorner)
                {
                    fourthBlock = Instantiate(blocks[Random.Range(0, blocks.Length - 2)], new Vector3(thirdBlock.transform.position.x + 4, 0, thirdBlock.transform.position.z), Quaternion.identity);
                    fourthBlock.transform.rotation = Quaternion.Euler(0, 90, 0);
                    direction = Direction.Rigth;
                }
                currentBlock = secondBlock;
            }
        }
        else if (direction == Direction.Rigth)
        {            
            Destroy(firstBlock, 3);
            firstBlock = secondBlock;
            secondBlock = thirdBlock;
            thirdBlock = fourthBlock;
               
            fourthBlock = Instantiate(blocks[4], new Vector3(thirdBlock.transform.position.x + 12, 0, thirdBlock.transform.position.z), Quaternion.identity);
            fourthBlock.transform.rotation = Quaternion.Euler(0, 90, 0);
           
            firstBlock = secondBlock;
            secondBlock = thirdBlock;
            thirdBlock = fourthBlock;

            fourthBlock = Instantiate(blocks[Random.Range(0, blocks.Length - 2)], new Vector3(thirdBlock.transform.position.x , 0, thirdBlock.transform.position.z + 4), Quaternion.identity);            

            currentBlock = secondBlock;
            direction = Direction.Forward;

            Block[] garbageBlocks = FindObjectsOfType<Block>();

            for (int i = 0; i < garbageBlocks.Length; i++)
            {
                if (garbageBlocks[i].transform.position.z < character.transform.position.z - 20)
                {
                    Destroy(garbageBlocks[i].gameObject);
                }
            }

        }
        else if (direction == Direction.Left)
        {
            if (character.transform.position.x <= currentBlock.transform.position.x)
            {
                Destroy(firstBlock, 3);
                firstBlock = secondBlock;
                secondBlock = thirdBlock;
                thirdBlock = fourthBlock;

                fourthBlock = Instantiate(blocks[5], new Vector3(thirdBlock.transform.position.x - 12, 0, thirdBlock.transform.position.z), Quaternion.identity);
                fourthBlock.transform.rotation = Quaternion.Euler(0, -90, 0);               

                currentBlock = secondBlock;
                direction = Direction.Forward;
                
                firstBlock = secondBlock;
                secondBlock = thirdBlock;
                thirdBlock = fourthBlock;

                fourthBlock = Instantiate(blocks[Random.Range(0, blocks.Length - 2)], new Vector3(thirdBlock.transform.position.x, 0, thirdBlock.transform.position.z + 4), Quaternion.identity);

                currentBlock = secondBlock;
                direction = Direction.Forward;

                Block[] garbageBlocks = FindObjectsOfType<Block>();

                for (int i = 0; i < garbageBlocks.Length; i++)
                {
                    if (garbageBlocks[i].transform.position.z < character.transform.position.z - 20)
                    {
                        Destroy(garbageBlocks[i].gameObject);
                    }
                }

            }
        }

       
    }

}
