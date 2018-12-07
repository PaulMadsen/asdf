using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadView : MonoBehaviour {

    [SerializeField]
    float moveSpeed = 10.0f;
    float mouseWheelMultiplier = 3;
    float shiftBoostSpeedMultiplier = 3;
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float shiftBoostSpeed = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            shiftBoostSpeed = shiftBoostSpeedMultiplier;
        if (Input.GetKey(KeyCode.A))
            transform.Translate(-moveSpeed * Time.deltaTime * shiftBoostSpeed, 0, 0);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(moveSpeed * Time.deltaTime * shiftBoostSpeed, 0, 0);
        if (Input.GetKey(KeyCode.W))
            transform.Translate(0, 0, moveSpeed * Time.deltaTime * shiftBoostSpeed);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(0, 0, -moveSpeed * Time.deltaTime * shiftBoostSpeed);
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            transform.Translate(0, moveSpeed * Time.deltaTime * mouseWheelMultiplier * shiftBoostSpeed, 0);
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            transform.Translate(0, -moveSpeed * Time.deltaTime * mouseWheelMultiplier * shiftBoostSpeed, 0);

    }
}
