using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// pairs a block id with all the information that block represents 
/// </summary>
public class BlockPair
{    
    public int blockID;
    public int meta; //placeholder, change to BlockMeta when its implemented
    public BlockPair(int blockID, int meta = 0)
    {
        this.blockID = blockID;
        this.meta = meta;
    }    
}

public class BlockMeta
{    
    //int BlockHealth = 0;
    //int orientation = 0; //bitmask represents rotation in 3 axis
    //int pieces = 0; //also bitmask.  How many pieces of the block exist
    
}

public class Block
{
    public static Dictionary<int, Block> BlockInfo = new Dictionary<int, Block>();
    public int textureXOffset;
    public int textureYOffset;
    public string displayName;
    public Block(int textureXOffset = 15, int textureYOffset = 15, string displayName = "texture not found")
    {
        this.textureXOffset = textureXOffset;
        this.textureYOffset = textureYOffset;
        this.displayName = displayName;
    }
}