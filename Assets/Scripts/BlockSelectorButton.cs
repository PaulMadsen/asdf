using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectorButton : MonoBehaviour {

    [SerializeField]
    Button button;
    public int blockID;
	void Start () {
        button.onClick.AddListener(ButtonClicked);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void ButtonClicked()
    {
        Debug.Log(button.GetComponentInChildren<Text>().text + " was clicked, ID: "+ blockID);
    }
}
