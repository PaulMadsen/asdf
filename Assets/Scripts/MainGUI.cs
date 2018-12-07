using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainGUI : MonoBehaviour {

    // Use this for initialization    
    [SerializeField]
    GameObject world;
	void Start () {
        //gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onSave()
    {
        StartCoroutine("DoSave");
    }
    public void onLoad()
    {
        Debug.Assert(world != null);
        foreach (Transform child in world.transform)
            GameObject.Destroy(child.gameObject);
        Chunk.allBlocks.Clear();
    }
    IEnumerator DoSave()
    {        
        BinaryWriter writer = new BinaryWriter(new FileStream("saveFile", FileMode.Create, FileAccess.Write, FileShare.None));
        writer.BaseStream.Seek(0, SeekOrigin.Begin);
        foreach (var pair in Chunk.allBlocks)
        {
            Chunk c = pair.Value;            
            writer.Write(Mathf.FloorToInt(pair.Key.x));
            writer.Write(Mathf.FloorToInt(pair.Key.y));
            c.SaveChunk(ref writer);
            yield return null;
        }
        writer.Close();
    }
}
