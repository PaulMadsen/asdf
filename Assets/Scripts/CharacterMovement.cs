using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    CharacterController cc;
    public float speed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = 20.0f;
    Vector3 moveDir = Vector3.zero;
    //bool grounded = false;
    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }
    void Update()
    {
        Vector3 myPos = transform.position - new Vector3(0, 1.08f, 0);

        /*if (World.GetBlock(myPos) > 0)
            grounded = true;
        else
            grounded = false;*/
        if (cc.isGrounded)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= speed;
            if (Input.GetButton("Jump"))
            {
                moveDir.y = jumpSpeed;
            }
        }

        Vector3 collisionCheck = moveDir.normalized;
        collisionCheck.y = 0;
        if (World.GetBlock(myPos + new Vector3((collisionCheck.x / 3), 0.5f, 0)) > 0)
            moveDir.x = 0;
        if (World.GetBlock(myPos + new Vector3(0, 0.5f, (collisionCheck.z / 3))) > 0)
            moveDir.z = 0;

        if (!cc.isGrounded)
            moveDir.y -= gravity * Time.deltaTime;
        cc.Move(moveDir * Time.deltaTime);
    }
}