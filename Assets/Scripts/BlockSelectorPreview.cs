using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSelectorPreview : MonoBehaviour
{

    public Material matPrefab;
    public GameObject self;
   

    void Start () {
        RectTransform GUIRectTransform = GetComponent<RectTransform>();
        float guiWidth = GUIRectTransform.rect.width;
        float guiHeight = GUIRectTransform.rect.height;
        Debug.Log(guiWidth + " " + guiHeight);
        Vector3 previewCubePos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        previewCubePos -= new Vector3(200f, 200f, 0);
        List<Vector3> previewPositions = new List<Vector3>();
        previewPositions.Add(GameObject.Find("p_cube0").transform.position);
        previewPositions.Add(GameObject.Find("p_cube1").transform.position);
        previewPositions.Add(GameObject.Find("p_cube2").transform.position);
        previewPositions.Add(GameObject.Find("p_cube3").transform.position);
        previewPositions.Add(GameObject.Find("p_cube4").transform.position);
        previewPositions.Add(GameObject.Find("p_cube5").transform.position);
        previewPositions.Add(GameObject.Find("p_cube6").transform.position);
        previewPositions.Add(GameObject.Find("p_cube7").transform.position);
        int positionIndex = 0;

        foreach (var b in Block.BlockInfo)
        {


            GameObject block = new GameObject(b.Value.displayName);
            block.transform.SetParent(this.transform);
            //block.transform.position = transform.position;
            block.transform.position = previewPositions[positionIndex++];
            MeshRenderer mr = block.AddComponent<MeshRenderer>();
            mr.material = matPrefab;
            MeshFilter mf = block.AddComponent<MeshFilter>();
            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            block.layer = 5; //UI
            

            triangles.Add(verts.Count + 0); //upper left triangle
            triangles.Add(verts.Count + 1);
            triangles.Add(verts.Count + 2);

            triangles.Add(verts.Count + 0); //lower right triangle
            triangles.Add(verts.Count + 2);
            triangles.Add(verts.Count + 3);

            verts.Add(new Vector3(0, 0, 0)); //lower left
            verts.Add(new Vector3(0, 1, 0)); //upper left
            verts.Add(new Vector3(1, 1, 0)); //uppr right
            verts.Add(new Vector3(1, 0, 0)); //lower right

            float blockWidth = (float)1 / (float)16;
            int xOffset = b.Value.textureXOffset;
            int yOffset = b.Value.textureYOffset;
            uvs.Add(new Vector2((0 + xOffset) * blockWidth, (0 + yOffset) * blockWidth));
            uvs.Add(new Vector2((0 + xOffset) * blockWidth, (1 + yOffset) * blockWidth));
            uvs.Add(new Vector2((1 + xOffset) * blockWidth, (1 + yOffset) * blockWidth));
            uvs.Add(new Vector2((1 + xOffset) * blockWidth, (0 + yOffset) * blockWidth));

            Mesh m = new Mesh();

            m.vertices = verts.ToArray();
            m.triangles = triangles.ToArray();
            m.uv = uvs.ToArray();
            m.RecalculateNormals();
            mf.mesh = m;
            //MeshCollider mc = block.AddComponent<MeshCollider>();
            //mc.sharedMesh = m;
            block.transform.localScale = new Vector3(100, 100, 100); //make it 100 times bigger
        }

        gameObject.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}    
}
