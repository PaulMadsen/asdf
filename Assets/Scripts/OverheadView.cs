using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadView : MonoBehaviour {

    [SerializeField]
    float moveSpeed = 10.0f;
    float mouseWheelMultiplier = 3;
    float shiftBoostSpeedMultiplier = 3;
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject mobContainer;
    public static List<GameObject> selectedUnits;
    
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
        if (Input.GetKeyDown(KeyCode.I))
            SpawnThing();
        if (Input.GetKeyDown(KeyCode.P))
            DamageThing();
        if (Input.GetMouseButtonDown(1))
            MoveUnits();
    }

    

    //prototype.  split up into constituent parts once working
    public void SpawnThing()
    {           
        Debug.Assert(characterPrefab != null);
        Camera cam = GetComponentInChildren<Camera>();
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            GameObject go = Instantiate(characterPrefab, hit.point + hit.normal, transform.rotation);
            go.transform.SetParent(mobContainer.transform);            
        }
    }

    public void DamageThing()
    {
        Debug.Assert(characterPrefab != null);
        Camera cam = GetComponentInChildren<Camera>();
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject go;
        if (Physics.Raycast(ray, out hit) && (go = hit.transform.gameObject).GetComponent<Humanoid>() ) 
        {
            go.GetComponent<Humanoid>().healthPoints -= 11f;
        }
    }

    void MoveUnits()
    {
        if (selectedUnits == null || selectedUnits.Count == 0) return;
        foreach (GameObject go in selectedUnits)
        {
            
        }
    }
}
