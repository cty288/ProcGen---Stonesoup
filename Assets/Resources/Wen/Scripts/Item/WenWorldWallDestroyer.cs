using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenWorldWallDestroyer : Tile
{
    public override void useAsItem(Tile tileUsingUs)
    {
        Wall[] wall = GameObject.FindObjectsOfType<Wall>(true);

        foreach (Wall w in wall)
        {
            w.takeDamage(w, 10000, DamageType.Explosive);
        }

        die();
    }
}
