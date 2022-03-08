using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Tile {
    public override void pickUp(Tile tilePickingUsUp) {
        base.pickUp(tilePickingUsUp);
        Debug.Log("Pick up");
        tilePickingUsUp.restoreAllHealth();
        die();
    }
}
