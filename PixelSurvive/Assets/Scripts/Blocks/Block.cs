using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Data/Create new Block")]
public class Block : ScriptableObject
{
    public BlockData blockData;
}
[System.Serializable]
public class BlockData
{
    public string typeId;
    public Texture2D texture;
    public float miningSpeed;
    public MiningTool blockType;
}
public enum MiningTool
{
    Pickaxe,
    Axe,
    Sword,
    Shovel,
    Hoe,
    None
}