using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : Tile
{
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        if (tilePickingUsUp.GetComponent<Player>().moveSpeed < 20)
        {
            tilePickingUsUp.GetComponent<Player>().moveSpeed += 2;
        }
        die();
    }
}
