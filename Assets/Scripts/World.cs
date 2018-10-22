using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour {
    private int WIDTH = 16;
    public static Transform self;
    
    [SerializeField]
    public Material mat; //plug-in in unity GUI
    public static Material mats; //script uses this one    

    // Use this for initialization
    void Start () {        

        mats = mat;
        self = transform;
        Chunk.allBlocks = new Dictionary<Vector2, Chunk>();        
        for (int chunkX = 0; chunkX < WIDTH; ++chunkX)
        {
            for (int chunkZ = 0; chunkZ < WIDTH; ++chunkZ)
            {
                GameObject chunk = new GameObject("Chunk " + "(" + chunkX + "," + chunkZ + ")");
                Chunk c = chunk.AddComponent<Chunk>();
                chunk.transform.parent = self;
                chunk.transform.position = new Vector3(chunkX * WIDTH, 0, chunkZ * WIDTH);
                Vector2 cPos = new Vector2(chunkX, chunkZ);                                
                Chunk.allBlocks.Add(cPos, c);
                chunk.AddComponent<MeshFilter>();
                chunk.AddComponent<MeshRenderer>();
            }
        }

        
	}
    
    void Awake()
    {
        Block.BlockInfo.Add(1, new Block(6, 15, "stone")); //for testing.  Load from file later
        Block.BlockInfo.Add(2, new Block(2, 15, "dirt"));
        Block.BlockInfo.Add(3, new Block(4, 15, "wood plank"));
        Block.BlockInfo.Add(4, new Block(1, 14, "stone brick"));
        Block.BlockInfo.Add(5, new Block(2, 14, "sand"));
        Block.BlockInfo.Add(6, new Block(3, 14, "bedrock"));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public static int GetBlock(Vector3 pos) {        
        Vector2 chunkPos = new Vector2((int)(pos.x / Chunk.CHUNK_WIDTH), (int)(pos.z / Chunk.CHUNK_WIDTH));        
        if (!Chunk.allBlocks.ContainsKey(chunkPos)) return 0;
        return Chunk.allBlocks[chunkPos].GetBlock(pos);
    }
}
