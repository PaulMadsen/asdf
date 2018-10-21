using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirticalLook : MonoBehaviour {
    public float sensitivity = 9.0f;
    public float minimumVert = -90.0f;
    public float maximumVert = 90.0f;
    public float _rotationX = 0;

	//controls only up and down looking
    //attach to camera only.
    //left and right mouse look would be controlled by the character object
	void Update () {
        _rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
        transform.localEulerAngles = new Vector3(_rotationX, 0, 0);
    }
}
