using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    public LayerMask lm;
    public GameObject bhPrefab;
    GameObject bh; //blockhighlighter


    void Awake()
    {
        bh = GameObject.Instantiate(bhPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, lm))
        {
            Vector3 blockPos = new Vector3(((int)hit.point.x) + 0.5f, ((int)hit.point.y) + 0.5f, ((int)hit.point.z) + 0.5f);
            bh.transform.position = blockPos;

            if (Input.GetMouseButtonDown(0))
            {

            }
        }
    }
}
