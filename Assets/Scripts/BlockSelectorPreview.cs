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
        float buttonLeftBound = -canvasWidth / 2 + 80.0f; //offsets from the canvas center
        float buttonRightBound = canvasWidth / 2 + 40.0f;
        float buttonTopBound = canvasHeight / 2 - 80.0f;        
        float buttonLowerBound = -canvasHeight / 2 + 40.0f;
        float hSpace = Mathf.Abs(buttonLeftBound) + Mathf.Abs(buttonRightBound);
        float vSpace = Mathf.Abs(buttonTopBound) + Mathf.Abs(buttonLowerBound);
        int numV_tiles = (int)(vSpace / (spaceBetween + cubeSize));
        int numH_tiles = (int)(hSpace / (spaceBetween + cubeSize));
        float scale = 32.0f / 512.0f; // for my particular .png
        int blockID = 1;
        for (int i=0; i<=numV_tiles; ++i)
        {
            for (int j=0; j<numH_tiles; ++j) {
                if (blockID >= Block.BlockInfo.Count) break;
                Button b = Instantiate(buttonPrefab);
                var buttonScript = b.GetComponent("BlockSelectorButton") as BlockSelectorButton;
                buttonScript.blockID = blockID;
                
                b.transform.position = this.transform.position;
                Block block = Block.BlockInfo[blockID++];
                b.transform.SetParent(this.transform);
                b.transform.name = block.displayName;
                b.GetComponentInChildren<Text>().text = block.displayName;
                b.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                b.transform.localPosition = new Vector3(buttonLeftBound + (j * (spaceBetween + cubeSize)), buttonTopBound - i * (spaceBetween + cubeSize), 0.0f);        
        
                Image img = b.GetComponent<Image>();            
                Material mat = new Material(img.material);
                mat.mainTextureScale = new Vector2(scale, scale);            
                mat.mainTextureOffset = new Vector2(block.textureXOffset * scale, block.textureYOffset * scale);
                img.material = mat;
            }
        }
        
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}    
}
