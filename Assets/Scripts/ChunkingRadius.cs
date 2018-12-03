using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamically loads or generates new chunks based on a radius around a player or other in game object.
/// Attach this to the player camera
/// </summary>
public class ChunkingRadius : MonoBehaviour {
    [SerializeField]
    public float chunkRadius = 2.0f;	


    private void Awake()
    {
       
    }

    // Update is called once per frame
    void Update () {
        Vector2 centerChunkPos = new Vector2(Mathf.FloorToInt(transform.position.x / Chunk.CHUNK_WIDTH), Mathf.FloorToInt(transform.position.z / Chunk.CHUNK_WIDTH));
        for (int i = Mathf.FloorToInt(centerChunkPos.x) - Mathf.FloorToInt(chunkRadius); i < Mathf.FloorToInt(centerChunkPos.x) + chunkRadius; ++i)
        {
            for (int j = Mathf.FloorToInt(centerChunkPos.y) - Mathf.FloorToInt(chunkRadius); j < Mathf.FloorToInt(centerChunkPos.y) + chunkRadius; ++j)
            {
                Vector2 targetChunk = new Vector2(i, j);
                float distance = Mathf.Sqrt(Mathf.Pow(targetChunk.x - centerChunkPos.x, 2) + Mathf.Pow(targetChunk.y - centerChunkPos.y, 2));
                if (distance > chunkRadius) continue;
                if (Chunk.allBlocks.ContainsKey(targetChunk)) continue;  //allready present
                //generate chunk
                World.InstantiateChunk(Mathf.FloorToInt(targetChunk.x), Mathf.FloorToInt(targetChunk.y));
            }
        }
    }
}
