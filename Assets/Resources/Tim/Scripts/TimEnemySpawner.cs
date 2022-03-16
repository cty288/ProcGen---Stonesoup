using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TimEnemySpawner : Tile {
    [SerializeField] private List<GameObject> normalEnemies;
    [SerializeField] private List<GameObject> rareEnemies;

    [SerializeField] private float standardSpawnInterval = 5f;
    [SerializeField] private int maxSpawnCount = 5;

    private List<Tile> aliveEnemies = new List<Tile>();
    private float timer = 0;
    
    private void Update() {
        timer += Time.deltaTime;
        if (timer >= standardSpawnInterval + Random.Range(-2f,2f)) {
            timer = 0;
            if (CheckCanSpawnEnemy()) {
                Tile spawnedTile = SpawnAEnemy();
                aliveEnemies.Add(spawnedTile);
            }
        }
    }
    
    private Tile SpawnAEnemy() {
        Vector2 gridCoord = toGridCoord(new Vector2(localX,localY));
        if (Random.Range(0, 100) >= 30) { //normal
            
            return  this.SpawnTile(GlobalFuncs.randElem(normalEnemies), transform.parent, (int) gridCoord.x , (int) gridCoord.y);
        }
        else { //rare
            return this.SpawnTile(GlobalFuncs.randElem(rareEnemies), transform.parent, (int)gridCoord.x, (int)gridCoord.y);
        }
    }

    private bool CheckCanSpawnEnemy() {
        //remove all dead enemy first
        aliveEnemies.RemoveAll(t => !t || t.health <= 0);
        return aliveEnemies.Count < maxSpawnCount;
    }
}
