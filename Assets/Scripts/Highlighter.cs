using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour {

    public GameObject highlighterPrefab;
    private GameObject highlighter;
	// Use this for initialization
	void Awake()
    {
        highlighter = GameObject.Instantiate(highlighterPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    }
        
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
