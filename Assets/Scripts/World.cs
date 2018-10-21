using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour {
    private int WIDTH = 16;
    public static Transform self;
    private static Dictionary<Vector2, Chunk> allBlocks;
    [SerializeField]
    public Material mat; //plug-in in unity GUI
    public static Material mats; //script uses this one

    // Use this for initialization
    void Start () {
        mats = mat;
        self = transform;
        allBlocks = new Dictionary<Vector2, Chunk>();        
        for (int chunkX = 0; chunkX < WIDTH; ++chunkX)
        {
            for (int chunkZ = 0; chunkZ < WIDTH; ++chunkZ)
            {
                GameObject chunk = new GameObject("Chunk " + "(" + chunkX + "," + chunkZ + ")");
                Chunk c = chunk.AddComponent<Chunk>();
                chunk.transform.parent = self;
                chunk.transform.position = new Vector3(chunkX * WIDTH, 0, chunkZ * WIDTH);
                Vector2 cPos = new Vector2(chunkX, chunkZ);                                
                allBlocks.Add(cPos, c);
                MeshFilter mf = chunk.AddComponent<MeshFilter>();
                MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
            }
        }

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static int GetBlock(Vector3 pos) {
        Vector2 chunkPos = new Vector2((int)(pos.x / Chunk.CHUNK_WIDTH), (int)(pos.z / Chunk.CHUNK_WIDTH));
        if (!allBlocks.ContainsKey(chunkPos)) return 0;
        return allBlocks[chunkPos].GetBlock(pos);
    }
}
