using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour {

    float defaultHealthPoints = 20f;
    public float healthPoints;
    // Use this for initialization
	void Start () {
        healthPoints = defaultHealthPoints;
	}
	
	// Update is called once per frame
	void Update () {
		if (healthPoints <= 0)
        {
            Rigidbody rigBody = GetComponent<Rigidbody>();
            if (rigBody != null) {
                rigBody.constraints = RigidbodyConstraints.None;
            }
            StartCoroutine("Death");
        }
	}

    IEnumerator Death() {
        float timeTillDestroy = 5f;
        while (timeTillDestroy > 0)
        {
            timeTillDestroy -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);        
    }
}

