using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBooster : Tile
{
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        tilePickingUsUp.AddMaximumHealth(1);
        die();
    }
}
