using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void onStandingPose()
    {
        Debug.Log("Standing Pose selected");
    }
    public void onCrouchPose()
    {
        Debug.Log("Crouch Pose selected");
    }
}
