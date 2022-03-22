using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : Tile
{
    public List<GameObject> TilesForGeneratingList;

    void RandomGenerate()
    {
        int index = Random.Range(0, TilesForGeneratingList.Count);
        TileExtension.SpawnTile(TilesForGeneratingList[index], transform.parent, transform.localPosition);
        Destroy(gameObject);
    }

    private void Start()
    {
        RandomGenerate();
    }
}
