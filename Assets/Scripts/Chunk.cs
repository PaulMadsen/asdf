using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public const int CHUNK_PIECES = 8; //total virtical chunk pieces in a chunk
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 16;  //virtical blocks per chunk piece
    private List<BlockPair[,,]> chunkSegments = new List<BlockPair[,,]>();
    private bool meshDirty = true; //mesh needs (re)generated?
    //each virtical chunk piece has meshes, verts, triangles and UVs
    private List<MeshFilter> meshes = new List<MeshFilter>();
    private List<MeshCollider> colliders = new List<MeshCollider>();
    private List<List<Vector3>> verts = new List<List<Vector3>>();
    private List<List<int>> triangles = new List<List<int>>();
    private List<List<Vector2>> uvs = new List<List<Vector2>>();
    public static Dictionary<Vector2, Chunk> allBlocks;

    void Start () {        
        System.Random randy = new System.Random();
        for (int i = 0; i < CHUNK_PIECES; ++i)
        {
            chunkSegments.Add(new BlockPair[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH]);
            GameObject chunkPiece = new GameObject("Chunk Piece " + i);
            meshes.Add(chunkPiece.AddComponent<MeshFilter>());
            MeshRenderer mr = chunkPiece.AddComponent<MeshRenderer>();
            colliders.Add(chunkPiece.AddComponent<MeshCollider>());
            mr.material = World.mats;
            chunkPiece.transform.position = new Vector3(transform.position.x, i * CHUNK_HEIGHT, transform.position.z);
            chunkPiece.transform.SetParent(this.transform);

            //generate default terrain
            for (int x = 0; x < CHUNK_WIDTH; ++x)           
                for (int y = 0; y < CHUNK_WIDTH; ++y)
                    for (int z = 0; z < CHUNK_WIDTH; ++z)
                    {
                        if (i > 1) continue; // chunkSegments[i][x, y, z] = new BlockPair(0);
                        if (i == 1 && y == 6) {

                            if (randy.Next(0, 100) == 1) chunkSegments[i][x, y, z] = new BlockPair(1);
                        }

                        if (i == 1 && y > 5)
                            continue; // chunkSegments[i][x, y, z] = new BlockPair(0); //cutting it short here
                        chunkSegments[i][x, y, z] = new BlockPair(1);
                    }            
        }

	}


    public static void SetBlock(Vector3 blockPos, int blockID)
    {
        Vector2 chunkPos = new Vector2((int)(blockPos.x / CHUNK_WIDTH), (int)(blockPos.z / CHUNK_WIDTH));        
        Chunk chunk = allBlocks[chunkPos];

        int localX = (int)(blockPos.x % CHUNK_WIDTH);
        int localY = (int)(blockPos.y % CHUNK_HEIGHT);
        int localZ = (int)(blockPos.z % CHUNK_WIDTH);                
        int segment = (int)(blockPos.y / CHUNK_HEIGHT);

        if (blockID == 0)
            chunk.chunkSegments[segment][localX, localY, localZ] = null;
        else
            chunk.chunkSegments[segment][localX, localY, localZ] = new BlockPair(blockID);

        /*for (int i = 0; i < 16; ++i)
            chunk.chunkSegments[segment][localX, i, localZ] = new BlockPair(3);*/
        chunk.meshDirty = true; //TODO make individual segments dirty, not the whole stack
        if (localX == 0)
        {
            Vector2 neihborPos = chunkPos - new Vector2(1, 0);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty = true;
        }
        else if (localX == CHUNK_WIDTH - 1)
        {
            Vector2 neihborPos = chunkPos + new Vector2(1, 0);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty = true;
        }
        if (localY == 0)
        {
            Vector2 neihborPos = chunkPos - new Vector2(0, 1);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty = true;
        }
        else if (localY == CHUNK_WIDTH - 1)
        {
            Vector2 neihborPos = chunkPos + new Vector2(0, 1);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty = true;
        }
    }

    void Awake()
    {
        transform.parent = World.self;
    }
	
	// Update is called once per frame
	void Update () {
		if (meshDirty)
        {
            Cmesh();
        }
	}

    void Cmesh()
    {
        triangles.Clear();
        verts.Clear();        
        uvs.Clear();

        for (int i=0; i < CHUNK_PIECES; ++i)
        {
            BlockPair[,,] chunkPiece = chunkSegments[i];
            for (int x=0; x<CHUNK_WIDTH; ++x)
                for(int y=0; y<CHUNK_HEIGHT; ++y)
                    for(int z=0; z<CHUNK_WIDTH; ++z)
                    {
                        if (chunkPiece[x, y, z] != null && 
                            chunkPiece[x,y,z].blockID > 0)                            
                        {
                            if (IsFaceVisible(x, y+1, z, i))
                                AddFace(x, y, z, i, FaceDirection.top);
                            if (IsFaceVisible(x, y - 1, z, i))
                                AddFace(x, y, z, i, FaceDirection.bottom);
                            if (IsFaceVisible(x-1, y, z, i))
                                AddFace(x, y, z, i, FaceDirection.left);
                            if (IsFaceVisible(x+1, y, z, i))
                                AddFace(x, y, z, i, FaceDirection.right);
                            if (IsFaceVisible(x, y, z-1, i))
                                AddFace(x, y, z, i, FaceDirection.front);
                            if (IsFaceVisible(x, y, z+1, i))
                                AddFace(x, y, z, i, FaceDirection.back);

                        }
                    }
            if (verts.Count <= i) continue;
            Mesh m = new Mesh();
            m.vertices = verts[i].ToArray();
            m.triangles = triangles[i].ToArray();
            m.uv = uvs[i].ToArray();
            m.RecalculateNormals();          
            meshes[i].mesh = m;
            colliders[i].sharedMesh = m;      
        }
        meshDirty = false;
        
    }

    private bool IsFaceVisible(int x, int y, int z, int segment)
    {
        
        if (x >= CHUNK_WIDTH) return true;
        if (x < 0) return true;
        if (y >= CHUNK_HEIGHT) return true;
        if (y < 0) return true;
        if (z >= CHUNK_WIDTH) return true;
        if (z < 0) return true;

        if (chunkSegments[segment][x, y, z] != null && 
            chunkSegments[segment][x, y, z].blockID > 0)
        {
            return false;
        }
        return true;
    }

    void AddFace(int x, int y, int z, int chunkSegment, FaceDirection direction)
    {
        if (verts.Count <= chunkSegment)
            verts.Add(new List<Vector3>());
        if (triangles.Count <= chunkSegment)
            triangles.Add(new List<int>());
        if (uvs.Count <= chunkSegment)
            uvs.Add(new List<Vector2>());

        float textureIndexSize = (float)1 / (float)16;

        int blockID = chunkSegments[chunkSegment][x, y, z].blockID;
        float xOffset = Block.BlockInfo[blockID].textureXOffset;
        float yOffset = Block.BlockInfo[blockID].textureYOffset;

        if (direction == FaceDirection.top)
        {
            //triangles and verts are done in a counter-intuitive order            
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);

            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 0));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));

        }
        else if (direction == FaceDirection.bottom)
        {                       
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 0));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        }
        else if (direction == FaceDirection.left)
        {
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);

            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 0));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        }
        else if (direction == FaceDirection.right)
        {
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 0));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        }
        else if (direction == FaceDirection.front)
        {
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);

            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 0));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 0));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        }
        else if (direction == FaceDirection.back)
        {
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 1);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            triangles[chunkSegment].Add(verts[chunkSegment].Count + 3);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 2);
            triangles[chunkSegment].Add(verts[chunkSegment].Count + 0);

            verts[chunkSegment].Add(new Vector3(x + 0, y + 0, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 0, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 1, z + 1));
            verts[chunkSegment].Add(new Vector3(x + 1, y + 0, z + 1));

            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs[chunkSegment].Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        }
    }

    public int GetBlock(Vector3 pos)
    {        
        int segment = (int)(pos.y  / CHUNK_HEIGHT);
        if (chunkSegments.Count < segment) return 0;
        int x = (int)(pos.x % CHUNK_WIDTH);
        int y = (int)(pos.y % CHUNK_HEIGHT);
        int z = (int)(pos.z % CHUNK_WIDTH);
        
        if (segment < 0 || segment >= chunkSegments.Count) return 0;
        if (chunkSegments[segment][x, y, z] == null) return 0;
        return chunkSegments[segment][x, y, z].blockID;
        
        
    }
}

enum FaceDirection
{    left, right, front, back, top, bottom  }

