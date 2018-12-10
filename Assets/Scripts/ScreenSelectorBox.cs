using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSelectorBox : MonoBehaviour {

    Camera cam;
    Vector2 boxStart;
    Vector2 boxEnd;
    public List<GameObject> selected;
    [SerializeField]
    GameObject population;
    [SerializeField]
    Material material;
    
    // Use this for initialization
	void Start () {
        cam = GetComponentInChildren < Camera > ();
        Debug.Assert(cam != null);
        Debug.Assert(population != null);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl)){            
            boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Debug.Log("Box start: " + boxStart);            

        }
        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
        {
            boxEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Debug.Log("Box end: " + boxEnd);
            foreach (Transform child in population.transform)
            {
                Debug.Log("Found " + child.gameObject.name + " at coord " + child.transform.position + "Screen: " + cam.WorldToScreenPoint(child.transform.position));
            }
            Debug.Log("these gameobjects are within the selection window");
            float xDist = Mathf.Abs(boxEnd.x - boxStart.x);
            float yDist = Mathf.Abs(boxEnd.y - boxStart.y);
            Vector2 lowerLeft = new Vector2(Mathf.Min(boxStart.x, boxEnd.x), Mathf.Min(boxStart.y, boxEnd.y));
            Rect box = new Rect(lowerLeft, new Vector2(xDist, yDist));
            
            Debug.Log("Rect box is " + box.xMin + "," + box.yMin + " to " + box.xMax + "," + box.yMax);
            foreach (Transform child in population.transform)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(child.transform.position);
                if (box.Contains(screenPos, true))
                {                    
                    Debug.Log(child.transform.position + " -> " + screenPos);
                }
            }
        }
	}
}
