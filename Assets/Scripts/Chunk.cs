﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;


public class Chunk : MonoBehaviour {

    public const int CHUNK_PIECES = 8; //total virtical chunk pieces in a chunk
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 16;  //virtical blocks per chunk piece
    private List<BlockPair[,,]> chunkSegments = null;
    public bool[] meshDirty = new bool[CHUNK_PIECES]; //mesh needs (re)generated?
    //each virtical chunk piece has meshes, verts, triangles and UVs
    private List<MeshFilter> meshes = new List<MeshFilter>(new MeshFilter[CHUNK_PIECES]);
    private List<MeshCollider> colliders = new List<MeshCollider>(new MeshCollider[CHUNK_PIECES]);
    private List<List<Vector3>> verts = new List<List<Vector3>>(new List<Vector3>[CHUNK_PIECES]);
    private List<List<int>> triangles = new List<List<int>>(new List<int>[CHUNK_PIECES]);
    private List<List<Vector2>> uvs = new List<List<Vector2>>(new List<Vector2>[CHUNK_PIECES]);
    public static Dictionary<Vector2, Chunk> allBlocks = new Dictionary<Vector2, Chunk>();

    void Start () {	}

    public void Init(ref BinaryReader reader, bool onDisk)
    {
        for (int i = 0; i < CHUNK_PIECES; ++i) //setup GameObject's meshes, colliders, etc
        {
            GameObject chunkPiece = new GameObject("Chunk Piece " + i);
            meshes[i] = chunkPiece.AddComponent<MeshFilter>();
            MeshRenderer mr = chunkPiece.AddComponent<MeshRenderer>();
            colliders[i] = chunkPiece.AddComponent<MeshCollider>();
            mr.material = World.mats;
            chunkPiece.transform.position = new Vector3(transform.position.x, i * CHUNK_HEIGHT, transform.position.z);
            chunkPiece.transform.SetParent(this.transform);
        }

        //Load terrain from file or generate it
        if (!onDisk) { 
            GenerateTerrain();            
        }
        else        
            LoadChunk(ref reader);
        MarkAllDirty();
    }

    void MarkAllDirty()
    {
        for (int i = 0; i < meshDirty.Length; ++i) meshDirty[i] = true;
    }

    /// <summary>
    /// Loads terrain from disk into memory
    /// The stream's seek position must be set the the beginning of terrain data
    /// </summary>
    /// <param name="binaryReader"></param>
    void LoadChunk(ref BinaryReader binaryReader)
    {
        List<BlockPair[,,]> segments = new List<BlockPair[,,]>();
        for (int segNum=0; segNum < CHUNK_PIECES; ++segNum)
        {
            BlockPair[,,] segment = new BlockPair[Chunk.CHUNK_WIDTH, Chunk.CHUNK_HEIGHT, Chunk.CHUNK_WIDTH];
            segments.Add(segment);
            for (int i = 0; i < CHUNK_WIDTH; ++i)
                for (int j = 0; j < CHUNK_HEIGHT; ++j)
                    for (int k = 0; k < CHUNK_WIDTH; ++k) {
                        int blockID = binaryReader.ReadInt32();
                        int meta = binaryReader.ReadInt32(); //this will change When MetaInfo class is implemented
                        if (blockID != 0)
                            segment[i, j, k] = new BlockPair(blockID, meta);
                    }
        }
        chunkSegments = segments;
    }

    public void SaveChunk(ref BinaryWriter br)
    {
        foreach (BlockPair[,,] segment in chunkSegments)
        {
            for (int i=0; i< CHUNK_WIDTH; ++i)
                for (int j=0; j< CHUNK_HEIGHT; ++j)
                    for (int k=0; k<CHUNK_WIDTH; ++k)
                    {
                        if (segment[i, j, k] == null)
                        {                            
                            br.Write(0);
                            br.Write(0);
                            continue;                            
                        }
                        br.Write(segment[i, j, k].blockID);
                        br.Write(segment[i, j, k].meta);
                    }
        }
        
        Debug.Log("Chunk.SaveChunk finished saving.  Write head at location " + br.BaseStream.Position );
    }
    

    /// <summary>
    /// Very basic random terrain generation
    /// Sets the chunk's terrain data and also returns the terrain
    /// </summary>    
    public List<BlockPair[,,]> GenerateTerrain()
    {
        List<BlockPair[,,]> segments = new List<BlockPair[,,]>();
        System.Random randy = new System.Random();
        for (int i = 0; i < CHUNK_PIECES; ++i)
        {
            segments.Add(new BlockPair[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH]);
            //generate default terrain
            for (int x = 0; x < CHUNK_WIDTH; ++x)
                for (int y = 0; y < CHUNK_HEIGHT; ++y)
                {
                    for (int z = 0; z < CHUNK_WIDTH; ++z)
                    {
                        if (i > 1) continue; // chunkSegments[i][x, y, z] = new BlockPair(0);
                        if (i == 1 && y == 6)
                        {

                            if (randy.Next(0, 100) == 1) segments[i][x, y, z] = new BlockPair(1);
                        }

                        if (i == 1 && y > 5)
                            continue; // chunkSegments[i][x, y, z] = new BlockPair(0); //cutting it short here
                        segments[i][x, y, z] = new BlockPair(1);
                    }
                }
        }
        chunkSegments = segments;
        return segments;
    }

    public static Vector3 GlobalToChunkGrid(Vector3 vec)
    {        
        int localX = Mathf.FloorToInt(vec.x % CHUNK_WIDTH);        
        int localZ = Mathf.FloorToInt(vec.z % CHUNK_WIDTH);
        if (localX < 0)
            localX += CHUNK_WIDTH;
        if (localZ < 0)
            localZ += CHUNK_WIDTH;
        return new Vector3(localX, Mathf.FloorToInt(vec.y % CHUNK_HEIGHT), localZ);
    }

    public static void SetBlock(Vector3 blockPos, int blockID)
    {
        Vector2 chunkPos = new Vector2(Mathf.FloorToInt(blockPos.x / CHUNK_WIDTH), Mathf.FloorToInt(blockPos.z / CHUNK_WIDTH));        
        Chunk chunk = allBlocks[chunkPos];
        Vector3 localPos = GlobalToChunkGrid(blockPos);        
        int segment = Mathf.FloorToInt(blockPos.y / CHUNK_HEIGHT);

        if (blockID == 0)
            chunk.chunkSegments[segment][Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = null;
        else
            chunk.chunkSegments[segment][Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = new BlockPair(blockID);

        
        chunk.meshDirty[segment] = true; 
        if (localPos.x == 0) //mark surrounding chunks dirty if bordering them
        {
            Vector2 neihborPos = chunkPos - new Vector2(1, 0);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty[segment] = true;
        }
        else if (localPos.x == CHUNK_WIDTH - 1)
        {
            Vector2 neihborPos = chunkPos + new Vector2(1, 0);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty[segment] = true;
        }
        if (localPos.y == 0)
        {
            Vector2 neihborPos = chunkPos - new Vector2(0, 1);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty[segment] = true;
        }
        else if (localPos.y == CHUNK_WIDTH - 1)
        {
            Vector2 neihborPos = chunkPos + new Vector2(0, 1);
            if (allBlocks.ContainsKey(neihborPos))
                allBlocks[neihborPos].meshDirty[segment] = true;
        }
    }

    void Awake()
    {
        transform.parent = World.self;
    }
	
	// Update is called once per frame
	void Update () {
        for (int i=0; i<CHUNK_PIECES; ++i)
		    if (meshDirty[i])
            {
                Cmesh(i);
            }
	}

    void Cmesh(int dirtyPiece)
    {   
        triangles[dirtyPiece] = new List<int>();
        verts[dirtyPiece] = new List<Vector3>();
        uvs[dirtyPiece] = new List<Vector2>();

        BlockPair[,,] chunkPiece = chunkSegments[dirtyPiece];
        for (int x=0; x<CHUNK_WIDTH; ++x)
            for(int y=0; y<CHUNK_HEIGHT; ++y)
                for(int z=0; z<CHUNK_WIDTH; ++z)
                {
                    if (chunkPiece[x, y, z] != null && 
                        chunkPiece[x,y,z].blockID > 0)                            
                    {
                        if (IsFaceVisible(x, y+1, z, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.top);
                        if (IsFaceVisible(x, y - 1, z, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.bottom);
                        if (IsFaceVisible(x-1, y, z, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.left);
                        if (IsFaceVisible(x+1, y, z, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.right);
                        if (IsFaceVisible(x, y, z-1, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.front);
                        if (IsFaceVisible(x, y, z+1, dirtyPiece))
                            AddFace(x, y, z, dirtyPiece, FaceDirection.back);

                    }
                }
        //if (verts.Count <= i) continue; ???
        Mesh m = new Mesh();
        m.vertices = verts[dirtyPiece].ToArray();
        m.triangles = triangles[dirtyPiece].ToArray();
        m.uv = uvs[dirtyPiece].ToArray();
        m.RecalculateNormals();          
        meshes[dirtyPiece].mesh = m;
        colliders[dirtyPiece].sharedMesh = m; 
        meshDirty[dirtyPiece] = false;
        //World.navMeshDirty = true;
        
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
        
        int segment = Mathf.FloorToInt(pos.y  / CHUNK_HEIGHT);
        if (chunkSegments.Count < segment) return 0;
        Vector3 localPos = GlobalToChunkGrid(pos);        
        if (segment < 0 || segment >= chunkSegments.Count)                    
            return 0;
        
        if (chunkSegments[segment][Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] == null)                    
            return 0;        
        
        return chunkSegments[segment][Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)].blockID;
        
        
    }
}

enum FaceDirection
{    left, right, front, back, top, bottom  }

