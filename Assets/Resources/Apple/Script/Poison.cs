using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Tile
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Debug.Log("Poison");
            collision.GetComponent<Player>().takeDamage(collision.GetComponent<Tile>(), 2); //player will get hurt while touching the posion
            Destroy(gameObject); //The poison will disappear after the pick up
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
