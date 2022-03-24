using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpZone : Tile
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            Debug.Log("SpeedUp");
            GetComponent<Player>().moveSpeed += 4;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
