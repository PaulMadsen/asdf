using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour {
    float moveSpeed = 8f;
    float rotationVelocity = 1f;
    List<Vector3> path;
    public void DoMove(List<Vector3> path)
    {
        this.path = path;
        this.path.Reverse();
        StartCoroutine("MoveTo");
    }
    IEnumerator MoveTo()
    {        
        Vector3 notOnFloor = new Vector3(.5f, .5f, .5f);        
        if (path == null)
        {
            Debug.Log("DoMove() path is null");
        }
        else
        {
            for (int i = 0; i < path.Count; ++i)
            {
                while (Vector3.Distance(transform.position, path[i] + notOnFloor) > 1f)
                {
                    
                    transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.right, path[i] + notOnFloor, rotationVelocity * Time.deltaTime, 1.0f));
                    
                    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //go.transform.position = path[i] + notOnFloor;
                    //Destroy(go, 1);
                    transform.position = Vector3.MoveTowards(transform.position, path[i] + notOnFloor, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}
