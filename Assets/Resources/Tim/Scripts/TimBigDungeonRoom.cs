using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimBigDungeonRoom : TimDungeonRoom{
    [SerializeField] private List<GameObject> enemyPrefabs;
    protected override float DungeonRandomness {
        get {
            return dungeonRandomness * 2;
        }
    }

    protected override float DungeonSpawnRandomness {
        get {
            return dungeonSpawnRandomness * 2;
        }
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        exitLocations = requiredExits.requiredExitLocations().ToList();
        Dir addedExit  = (Dir)Random.Range(0, 4);
        requiredExits.addDirConstraint(addedExit);
        roomManager.SetAdditionalExitsForNeighbours(new Vector2Int(roomGridX, roomGridY), addedExit);

        base.fillRoom(ourGenerator, requiredExits);


        SpawnRandomEnemy();
    }

    private void SpawnRandomEnemy()
    {
        List<Vector2Int> availableGrids = new List<Vector2Int>();

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if (roomGrids[i, j] == 0 && i != LevelGenerator.ROOM_WIDTH / 2 && j != LevelGenerator.ROOM_HEIGHT / 2)
                {
                    availableGrids.Add(new Vector2Int(i, j));
                }
            }
        }

        int enemyNum = Random.Range(1, 4);

        
        if (availableGrids.Count > enemyNum)
        {
            for (int i = 0; i < enemyNum; i++) {
                Vector2Int spawnPos = availableGrids[Random.Range(0, availableGrids.Count)];
                GameObject spawnPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Tile.spawnTile(spawnPrefab, transform, spawnPos.x, spawnPos.y);
            }
        }

    }
}
