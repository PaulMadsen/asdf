using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class World : MonoBehaviour {
    private static int WIDTH = 16;
    public static Transform self;
    
    [SerializeField]
    public Material mat; //plug-in in unity GUI
    public static Material mats; //script uses this one    

    public static string saveFile = "saveFile";

    // Use this for initialization
    void Start () {
        FileStream fs;
        if (!File.Exists(saveFile)) { 
            fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            fs.Close();
        }
        Cursor.visible = false;
    }

    /// <summary>
    /// Loads or generates a entire chunk stack
    /// </summary>
    /// <param name="x">coordinate</param>
    /// <param name="y">coordinate</param>
    public static void InstantiateChunk(int x, int z)
    {
        GameObject chunkStack = new GameObject("Chunk " + "(" + x + "," + z + ")");
        Chunk chunk = chunkStack.AddComponent<Chunk>();
        BinaryReader reader = new BinaryReader(new FileStream(saveFile, FileMode.Open, FileAccess.Read , FileShare.Read));
        if (ChunkExistsOnDisk(ref reader, x, z)) {            
            chunk.Init(ref reader, true);            
        }
        else {            
            chunk.Init(ref reader, false);
        }
        reader.Close();
        chunkStack.transform.parent = self;
        chunkStack.transform.position = new Vector3(x * WIDTH, 0, z * WIDTH);
        Vector2 cPos = new Vector2(x, z);
        Chunk.allBlocks.Add(cPos, chunk);
        chunkStack.AddComponent<MeshFilter>();
        chunkStack.AddComponent<MeshRenderer>();
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public static int GetBlock(Vector3 pos) {        
        Vector2 chunkPos = new Vector2(Mathf.Floor(pos.x / Chunk.CHUNK_WIDTH), Mathf.Floor(pos.z / Chunk.CHUNK_WIDTH));
        if (!Chunk.allBlocks.ContainsKey(chunkPos)) return 0;
        return Chunk.allBlocks[chunkPos].GetBlock(pos);
    }

    /// <summary>
    /// The BinaryWriter's internal read head will be set to the beginning of data or garbage if the chunk is not found    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>True if the chunk exists on disk</returns>
    static private bool ChunkExistsOnDisk(ref BinaryReader br, int xCoord, int yCoord)
    {        
        int index = 0;
        int chunkSize = ((Chunk.CHUNK_WIDTH * Chunk.CHUNK_HEIGHT * Chunk.CHUNK_WIDTH * Chunk.CHUNK_PIECES) * 2 +2) * sizeof(int);

        while (br.BaseStream.Position <= br.BaseStream.Length)
        {
            br.BaseStream.Seek(index * chunkSize, SeekOrigin.Begin);
            if (br.BaseStream.Position >= br.BaseStream.Length) break;
            int x = br.ReadInt32();
            int y = br.ReadInt32();
            bool found = (x == xCoord && y == yCoord);            
            if (found)
            {
                return true;
            }
            ++index;
        }
        return false;
    }

    
}
