using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    public LayerMask lm;
    public GameObject bhPrefab;
    GameObject bh; //blockhighlighter
    public static int currentBlockID = 0;


    void Awake()
    {
        bh = GameObject.Instantiate(bhPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, lm))
        {
            if (!GUIstate.GUIActive)
            {
                //Vector3 blockPos = new Vector3(((int)hit.point.x) + 0.5f, ((int)hit.point.y) + 0.5f, ((int)hit.point.z) + 0.5f);
                Vector3 blockPos = hit.point - hit.normal / 2;
                bh.transform.position = new Vector3(Mathf.Floor(blockPos.x) + 0.5f, 
                Mathf.Floor(blockPos.y) + 0.5f, Mathf.Floor(blockPos.z) + 0.5f);
            
                if (Input.GetMouseButtonDown(0))
                {                
                    int blockID = World.GetBlock(blockPos);                
                    if (blockID != 0)
                    {
                        Chunk.SetBlock(blockPos, 0);
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {                    
                    blockPos += hit.normal; //block location in front of raycast hit
                    int blockID = World.GetBlock(blockPos);                
                    //if (blockID != 0)                    
                    Chunk.SetBlock(blockPos, currentBlockID);
                    
                }
            }
        }
    }
}
