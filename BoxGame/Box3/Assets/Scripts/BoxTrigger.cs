using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "push")
        {
            Moveright();
        }
    }

    void Moveright()
    {
        float T = 100 * Time.deltaTime;
        Vector3Int Target = new Vector3Int(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y), 0);
        transform.position = Vector3.MoveTowards(transform.position, Target, T);
    }

}
