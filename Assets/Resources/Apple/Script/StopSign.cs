using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSign : Tile
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            Debug.Log("Stop");
            collider.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
