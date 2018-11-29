using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public const int CHUNK_PIECES = 8; //total virtical chunk pieces in a chunk
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 8;  //virtical blocks per chunk piece
    private List<BlockPair[,,]> chunkSegments = new List<BlockPair[,,]>();
    public bool[] meshDirty = new bool[CHUNK_PIECES]; //mesh needs (re)generated?
    //each virtical chunk piece has meshes, verts, triangles and UVs
    private List<MeshFilter> meshes = new List<MeshFilter>(new MeshFilter[CHUNK_PIECES]);
    private List<MeshCollider> colliders = new List<MeshCollider>(new MeshCollider[CHUNK_PIECES]);
    private List<List<Vector3>> verts = new List<List<Vector3>>(new List<Vector3>[CHUNK_PIECES]);
    private List<List<int>> triangles = new List<List<int>>(new List<int>[CHUNK_PIECES]);
    private List<List<Vector2>> uvs = new List<List<Vector2>>(new List<Vector2>[CHUNK_PIECES]);
    public static Dictionary<Vector2, Chunk> allBlocks = new Dictionary<Vector2, Chunk>();

    void Start () {        
        System.Random randy = new System.Random();
        for (int i = 0; i < CHUNK_PIECES; ++i)
        {
            chunkSegments.Add(new BlockPair[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH]);
            GameObject chunkPiece = new GameObject("Chunk Piece " + i);
            meshes[i] = chunkPiece.AddComponent<MeshFilter>();
            MeshRenderer mr = chunkPiece.AddComponent<MeshRenderer>();
            colliders[i] = chunkPiece.AddComponent<MeshCollider>();
            mr.material = World.mats;
            chunkPiece.transform.position = new Vector3(transform.position.x, i * CHUNK_HEIGHT, transform.position.z);
            chunkPiece.transform.SetParent(this.transform);

            //generate default terrain
            for (int x = 0; x < CHUNK_WIDTH; ++x)           
                for (int y = 0; y < CHUNK_HEIGHT; ++y) {
                    meshDirty[i] = true;
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

	}

    public static Vector3 GlobalToChunkGrid(Vector3 vec)
    {        
        int localX = Mathf.FloorToInt(vec.x % CHUNK_WIDTH);        
        int localZ = Mathf.FloorToInt(vec.z % CHUNK_WIDTH);
        if (localX < 0)
            localX += CHUNK_WIDTH;
        if (localZ < 0)
            localZ += CHUNK_WIDTH;
        return new Vector3(localX, (int)(vec.y % CHUNK_HEIGHT), localZ);
    }

    public static void SetBlock(Vector3 blockPos, int blockID)
    {
        Vector2 chunkPos = new Vector2(Mathf.FloorToInt(blockPos.x / CHUNK_WIDTH), Mathf.FloorToInt(blockPos.z / CHUNK_WIDTH));        
        Chunk chunk = allBlocks[chunkPos];
        Vector3 localPos = GlobalToChunkGrid(blockPos);
        Debug.Log("Setting block at location " + localPos);
        int segment = (int)(blockPos.y / CHUNK_HEIGHT);

        if (blockID == 0)
            chunk.chunkSegments[segment][(int)localPos.x, (int)localPos.y, (int)localPos.z] = null;
        else
            chunk.chunkSegments[segment][(int)localPos.x, (int)localPos.y, (int)localPos.z] = new BlockPair(blockID);

        
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
        try { 
            triangles[dirtyPiece] = new List<int>();
            verts[dirtyPiece] = new List<Vector3>();
            uvs[dirtyPiece] = new List<Vector2>();
        }
        catch (Exception e){

            Debug.Log(dirtyPiece + " " + triangles.Count + " " + verts.Count + " " + uvs.Count);
        }



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
        
        int segment = (int)(pos.y  / CHUNK_HEIGHT);
        if (chunkSegments.Count < segment) return 0;
        Vector3 localPos = GlobalToChunkGrid(pos);
        Debug.Log("Global: (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
        Debug.Log("Local: (" + localPos.x + ", " + (int)localPos.y + ", " + localPos.z + ")");
        Debug.Log("Segment: " + segment);
        
        if (segment < 0 || segment >= chunkSegments.Count) return 0;
        if (chunkSegments[segment][(int)localPos.x, (int)localPos.y, (int)localPos.z] == null) return 0;
        return chunkSegments[segment][(int)localPos.x, (int)localPos.y, (int)localPos.z].blockID;
        
        
    }
}

enum FaceDirection
{    left, right, front, back, top, bottom  }

