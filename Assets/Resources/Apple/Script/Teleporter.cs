using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : Tile
{
    [SerializeField] private Transform destination;

    public Transform GetDestination()
    {
        return destination;
    }
}


