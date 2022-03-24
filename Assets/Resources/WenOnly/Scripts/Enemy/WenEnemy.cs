using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenEnemy : Tile
{
    public Player player;

    public void InitializeEnemy()
    {
        player = GameObject.Find("player_tile(Clone)").GetComponent<Player>();
    }

    public bool Detect(GameObject target, float detectRadius)
    {
        if ((transform.position - target.transform.position).magnitude <= detectRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
