using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamically loads or generates new chunks based on a radius around a player or other in game object.
/// Attach this to the player camera
/// </summary>
public class ChunkingRadius : MonoBehaviour {
    [SerializeField]
    public float chunkRadius = 1.0f;	


    private void Awake()
    {
       
    }

    // Update is called once per frame
    void Update () {
        Vector2 centerChunkPos = new Vector2((int)(transform.position.x / Chunk.CHUNK_WIDTH), (int)(transform.position.y / Chunk.CHUNK_WIDTH));
        for (int i = (int)Mathf.Abs(centerChunkPos.x) - (int)chunkRadius; i < (int)centerChunkPos.x + chunkRadius; ++i)
        {
            for (int j = (int)Mathf.Abs(centerChunkPos.y) - (int)chunkRadius; j < (int)centerChunkPos.y + chunkRadius; ++j)
            {
                Vector2 targetChunk = new Vector2(i, j);
                float distance = Mathf.Sqrt(Mathf.Pow(targetChunk.x - centerChunkPos.x, 2) + Mathf.Pow(targetChunk.x - centerChunkPos.x, 2));
                if (distance > chunkRadius) continue;
                if (Chunk.allBlocks.ContainsKey(targetChunk)) continue;  //allready present
                //generate chunk
                World.InstantiateChunk((int)targetChunk.x, (int)targetChunk.y);
            }
        }
    }
}
