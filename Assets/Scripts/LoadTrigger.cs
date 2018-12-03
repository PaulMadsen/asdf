using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTrigger : MonoBehaviour
{

    [SerializeField]
    Button self;
    [SerializeField]
    GameObject worldChunkContainer;
    // Use this for initialization
    void Start()
    {
        self.onClick.AddListener(Load);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Load()
    {
        StartCoroutine("DoLoad");
    }

    IEnumerator DoLoad()
    {
        Debug.Log("load triggered");

        foreach (Transform child in worldChunkContainer.transform)
            Destroy(child.gameObject);
        Chunk.allBlocks.Clear();
        
        yield return null;


    }
}
