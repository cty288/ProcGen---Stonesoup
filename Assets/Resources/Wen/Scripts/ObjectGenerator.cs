using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : Tile
{
    public List<GameObject> HighPossibilityTilesForGeneratingList;
    public List<GameObject> LowPossibilityTilesForGeneratingList;

    void RandomGenerate() {
        GameObject objToSpawn;
        if (Random.Range(0, 100) >= 30) {
            objToSpawn =
                HighPossibilityTilesForGeneratingList[Random.Range(0, HighPossibilityTilesForGeneratingList.Count)];
        }
        else {
            objToSpawn =
                LowPossibilityTilesForGeneratingList[Random.Range(0, LowPossibilityTilesForGeneratingList.Count)];
        }
      
        TileExtension.SpawnTile(objToSpawn, transform.parent, transform.localPosition);
        Destroy(gameObject);
    }

    private void Start()
    {
        RandomGenerate();
    }
}
