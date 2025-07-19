using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockType { Straight, LeftCorner, RightCorner }
    public BlockType blockType;
}
