using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockSelectorPreview : MonoBehaviour
{

    public Material matPrefab;
    public GameObject self;
    public float spaceBetween = 50.0f;
    public float cubeSize = 100.0f;
    [SerializeField]
    Button buttonPrefab;
   

    void Start () {
        RectTransform GUIRectTransform = GetComponent<RectTransform>();
        float canvasWidth = GUIRectTransform.rect.width;
        float canvasHeight = GUIRectTransform.rect.height;
        float buttonLeftBound = -canvasWidth / 2 + 40.0f; //offsets from the canvas center
        float buttonRightBound = canvasWidth / 2 + 50.0f;
        float hSpace = Mathf.Abs(buttonLeftBound) + Mathf.Abs(buttonRightBound);
        int numH_tiles = (int)(hSpace / (spaceBetween + cubeSize));
        float scale = 32.0f / 512.0f; // for my particular .png
        for (int i=1; i<=numH_tiles; ++i)
        {
            Button b = Instantiate(buttonPrefab);
            b.transform.position = this.transform.position;
            Block block = Block.BlockInfo[3];
            b.transform.SetParent(this.transform);
            b.transform.name = block.displayName;
            b.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);        
        
        
            Image img = b.GetComponent<Image>();            
            Material mat = new Material(img.material);
            mat.mainTextureScale = new Vector2(scale, scale);            
            mat.mainTextureOffset = new Vector2(block.textureXOffset * scale, block.textureYOffset * scale);
            img.material = mat;
        }
        
        gameObject.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}    
}
