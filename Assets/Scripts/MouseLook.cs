using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 9.0f;       
   
   
   
    void Update()
    {   
        float delta = Input.GetAxis("Mouse X") * sensitivity;
        float rotationY = transform.localEulerAngles.y + delta;
        transform.localEulerAngles = new Vector3(0, rotationY, 0);
        
    }
}