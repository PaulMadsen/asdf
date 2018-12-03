using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveTrigger : MonoBehaviour {

    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.G))
            StartCoroutine("DoSave");
        
	}


    IEnumerator DoSave()
    {

        Debug.Log("save triggered");
        BinaryWriter writer = new BinaryWriter(new FileStream("saveFile", FileMode.Create, FileAccess.Write, FileShare.None));
        writer.BaseStream.Seek(0, SeekOrigin.Begin);
        foreach (var pair in Chunk.allBlocks)
        {
            Chunk c = pair.Value;
            Debug.Log("saving chunk " + pair.Key.x + "," + pair.Key.y);
            writer.Write(Mathf.FloorToInt(pair.Key.x));
            writer.Write(Mathf.FloorToInt(pair.Key.y));
            c.SaveChunk(ref writer);
            yield return null;
        }
        writer.Close();
        Debug.Log("Finished saving all chunks");

    }

}
