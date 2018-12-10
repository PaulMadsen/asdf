using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetNeighbors : MonoBehaviour {

    Vector3 from = Vector3.zero; //starting point for A*
    
    [SerializeField]
    Camera cam;
    [SerializeField]
    Canvas DebugCanvas;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {                
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;                
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 target = new Vector3(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y), Mathf.Floor(hit.point.z));
                    var neihbors = AStar.GetNeihbors(new Vec3(target));
                    Debug.Log("Found " + neihbors.Count + " neihbors");
                    foreach (Vec3 neighbor in neihbors)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        go.transform.position = neighbor.theVector + new Vector3(0.5f, 0.5f, 0.5f);
                        go.transform.localScale *= 1.5f;
                    }
                }
                
            }
        }*/
        if (Input.GetKeyDown(KeyCode.F)){
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (from == Vector3.zero)
                {
                    from = new Vector3(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y), Mathf.Floor(hit.point.z));
                }
                else
                {
                    Vector3 to = new Vector3(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y), Mathf.Floor(hit.point.z));
                    var ret = AStar.A_Star(from, to, DebugCanvas);
                    foreach (Vector3 point in ret)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        go.transform.position = point + new Vector3(0.5f, 0.5f, 0.5f);
                        go.transform.localScale *= .5f;
                        Destroy(go, 3);
                    }
                    from = Vector3.zero; //reset for next use
                }
                
            }
        }

    }

}
