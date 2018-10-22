using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIstate : MonoBehaviour {
    
    [SerializeField]
    public GameObject BlockSelectorGUI;
    public static bool GUIActive;

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
        }
    }
}
