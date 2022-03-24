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
            GetComponent<Player>().moveSpeed = 0;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
