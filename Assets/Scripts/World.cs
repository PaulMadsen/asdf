using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour {
    private static int WIDTH = 16;
    public static Transform self;
    
    [SerializeField]
    public Material mat; //plug-in in unity GUI
    public static Material mats; //script uses this one    

    // Use this for initialization
    void Start () {        

      

        
	}

    /// <summary>
    /// Loads or generates a entire chunk stack
    /// </summary>
    /// <param name="x">coordinate</param>
    /// <param name="y">coordinate</param>
    public static void InstantiateChunk(int x, int z)
    {
        GameObject chunk = new GameObject("Chunk " + "(" + x + "," + z + ")");
        Chunk c = chunk.AddComponent<Chunk>();
        chunk.transform.parent = self;
        chunk.transform.position = new Vector3(x * WIDTH, 0, z * WIDTH);
        Vector2 cPos = new Vector2(x, z);
        Chunk.allBlocks.Add(cPos, c);
        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
    }

    void Awake()
    {
        Block.BlockInfo.Add(1, new Block(6, 15, "stone")); //for testing.  Load from file later
        Block.BlockInfo.Add(2, new Block(2, 15, "dirt"));
        Block.BlockInfo.Add(3, new Block(4, 15, "wood plank"));
        Block.BlockInfo.Add(4, new Block(1, 14, "stone brick"));
        Block.BlockInfo.Add(5, new Block(2, 14, "sand"));
        Block.BlockInfo.Add(6, new Block(3, 14, "bedrock"));
        mats = mat;
        self = transform;

        /*for (int chunkX = 0; chunkX < WIDTH; ++chunkX)
        {
            for (int chunkZ = 0; chunkZ < WIDTH; ++chunkZ)
            {
                InstantiateChunk(chunkX, chunkZ);
            }
        }*/

        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public static int GetBlock(Vector3 pos) {        
        Vector2 chunkPos = new Vector2((int)(pos.x / Chunk.CHUNK_WIDTH), (int)(pos.z / Chunk.CHUNK_WIDTH));
        Debug.Log("GetBlock at global: " + pos);
        Debug.Log("GetBlock chunk coord: " + chunkPos);
        if (!Chunk.allBlocks.ContainsKey(chunkPos)) return 0;
        return Chunk.allBlocks[chunkPos].GetBlock(pos);
    }
}
