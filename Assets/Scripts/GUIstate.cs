using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIstate : MonoBehaviour {
    
    [SerializeField]
    public GameObject BlockSelectorGUI;
    public static bool GUIActive = false;
    [SerializeField]
    GameObject mainMenuGUI;

    void Start()
    {
        GUIActive = BlockSelectorGUI.activeSelf;
    }
    public static bool IsActive()
    {
        return GUIActive;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BlockSelectorGUI.SetActive(!BlockSelectorGUI.activeSelf);
            GUIActive = BlockSelectorGUI.activeSelf;
            if (GUIActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else { 
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Assert(mainMenuGUI != null);
            Debug.Log("Escape pressed");
            mainMenuGUI.SetActive(!mainMenuGUI.activeSelf);

        }
    }
}
