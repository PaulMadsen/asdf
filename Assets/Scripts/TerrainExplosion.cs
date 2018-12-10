using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TerrainExplosion : MonoBehaviour {

    [SerializeField]
    Camera cam;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {            
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Explode(hit.point, 5);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backslash))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MakeMound(hit.point, 5);
            }
        }
            
		
	}

    void Explode(Vector3 center, int radius)
    {
        for (int x = Mathf.FloorToInt(center.x) - radius; x < Mathf.FloorToInt(center.x) + radius; ++x)
            for (int y = Mathf.FloorToInt(center.y) - radius; y < Mathf.FloorToInt(center.y) + radius; ++y)
                for (int z = Mathf.FloorToInt(center.z) - radius; z < Mathf.FloorToInt(center.z) + radius; ++z)
                {
                    Vector3 blockCoord = new Vector3(x, y, z);
                    if (Vector3.Distance(blockCoord, center) <= radius)
                    {
                        int blockID = World.GetBlock(blockCoord);
                        if (blockID  != 0) {
                            SpawnFlyingBlock(blockCoord, blockID);
                            Chunk.SetBlock(blockCoord, 0);
                        }
                    }
                }
    }

    void MakeMound(Vector3 center, int radius)
    {
        System.Random rand = new System.Random();
        int fillBlockID = rand.Next(0, Block.BlockInfo.Count);
        for (int x = Mathf.FloorToInt(center.x) - radius; x < Mathf.FloorToInt(center.x) + radius; ++x)
            for (int y = Mathf.FloorToInt(center.y) - radius; y < Mathf.FloorToInt(center.y) + radius; ++y)
                for (int z = Mathf.FloorToInt(center.z) - radius; z < Mathf.FloorToInt(center.z) + radius; ++z)
                {
                    Vector3 blockCoord = new Vector3(x, y, z);
                    if (Vector3.Distance(blockCoord, center) <= radius)
                    {
                        if (World.GetBlock(blockCoord) == 0)
                            Chunk.SetBlock(blockCoord, fillBlockID);
                    }
                }
    }

    void SpawnFlyingBlock(Vector3 startPos, int blockID)
    {
        float textureIndexSize = (float)1 / (float)16;
        float xOffset = Block.BlockInfo[blockID].textureXOffset;
        float yOffset = Block.BlockInfo[blockID].textureYOffset;
        List<int> triangles = new List<int>();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs  = new List<Vector2>();
        int x = Mathf.FloorToInt (startPos.y);
        int y = Mathf.FloorToInt(startPos.y);
        int z = Mathf.FloorToInt(startPos.z);

        //triangles and verts are done in a counter-intuitive order                        
            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 2);

            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 3);

            verts.Add(new Vector3(x + 0, y + 1, z + 0));
            verts.Add(new Vector3(x + 0, y + 1, z + 1));
            verts.Add(new Vector3(x + 1, y + 1, z + 1));
            verts.Add(new Vector3(x + 1, y + 1, z + 0));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));

      
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 0);

            triangles.Add(verts.Count + 3);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 0);

            verts.Add(new Vector3(x + 0, y + 0, z + 0));
            verts.Add(new Vector3(x + 0, y + 0, z + 1));
            verts.Add(new Vector3(x + 1, y + 0, z + 1));
            verts.Add(new Vector3(x + 1, y + 0, z + 0));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
     
            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 2);

            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 3);

            verts.Add(new Vector3(x + 0, y + 0, z + 1));
            verts.Add(new Vector3(x + 0, y + 1, z + 1));
            verts.Add(new Vector3(x + 0, y + 1, z + 0));
            verts.Add(new Vector3(x + 0, y + 0, z + 0));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
       
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 0);

            triangles.Add(verts.Count + 3);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 0);

            verts.Add(new Vector3(x + 1, y + 0, z + 1));
            verts.Add(new Vector3(x + 1, y + 1, z + 1));
            verts.Add(new Vector3(x + 1, y + 1, z + 0));
            verts.Add(new Vector3(x + 1, y + 0, z + 0));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
       
            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 2);

            triangles.Add(verts.Count + 0);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 3);

            verts.Add(new Vector3(x + 0, y + 0, z + 0));
            verts.Add(new Vector3(x + 0, y + 1, z + 0));
            verts.Add(new Vector3(x + 1, y + 1, z + 0));
            verts.Add(new Vector3(x + 1, y + 0, z + 0));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));

            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 0);

            triangles.Add(verts.Count + 3);
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 0);

            verts.Add(new Vector3(x + 0, y + 0, z + 1));
            verts.Add(new Vector3(x + 0, y + 1, z + 1));
            verts.Add(new Vector3(x + 1, y + 1, z + 1));
            verts.Add(new Vector3(x + 1, y + 0, z + 1));

            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((0 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (1 + yOffset) * textureIndexSize));
            uvs.Add(new Vector2((1 + xOffset) * textureIndexSize, (0 + yOffset) * textureIndexSize));
        GameObject flyingBlock = new GameObject("Flying block");
        Destroy(flyingBlock, 5);
        MeshFilter mf = flyingBlock.AddComponent<MeshFilter>();
        MeshRenderer mr = flyingBlock.AddComponent<MeshRenderer>();
        MeshCollider mc = flyingBlock.AddComponent<MeshCollider>();
        mr.material = World.mats;
        flyingBlock.transform.position = startPos;

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = triangles.ToArray();
        m.uv = uvs.ToArray();
        m.RecalculateNormals();
        
        mf.mesh = m;        
        mc.sharedMesh = m;
        mc.convex = true;
        Rigidbody rd = flyingBlock.AddComponent<Rigidbody>();
        rd.isKinematic = false;
        rd.useGravity = true;
        rd.AddExplosionForce(20f, flyingBlock.transform.position + new Vector3(0, -1f, 0), 5);
                
    }
}
