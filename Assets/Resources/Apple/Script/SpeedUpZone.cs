using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpZone : Tile
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -20;
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("SpeedUp");
            collider.GetComponent<Player>().moveSpeed += 4;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("SpeedUp");
            collider.GetComponent<Player>().moveSpeed -= 4;
        }
    }
}
