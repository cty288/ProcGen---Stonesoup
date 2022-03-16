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

        SpawnEnemySpawner();
        SpawnRandomEnemy();
        SpawnRat();
    }
    private void SpawnEnemySpawner()
    {
        List<Vector2Int> openAreas = GetOpenAreas(2);
        if (openAreas.Count > 0 && Random.Range(0, 100) <= 100)
        {
            Vector2Int openArea = GlobalFuncs.randElem(openAreas);
            Tile.spawnTile(enemySpawnerPrefab, transform, openArea.x, openArea.y);
            roomGrids[openArea.x, openArea.y] = 3;
        }
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

        int enemyNum = Random.Range(2, 5);
        int remainingGrids = availableGrids.Count;
        
        if (availableGrids.Count > enemyNum)
        {
            for (int i = 0; i < enemyNum; i++) {
                Vector2Int spawnPos = availableGrids[Random.Range(0, remainingGrids--)];
                GameObject spawnPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Tile.spawnTile(spawnPrefab, transform, spawnPos.x, spawnPos.y);
            }
        }

    }

    private List<Vector2Int> GetOpenAreas(int wallNeighbourMaxCount = 4)
    {
        List<Vector2Int> openAreaGrids = new List<Vector2Int>();

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if (roomGrids[i, j] <= 0)
                {
                    if (GetWallNeighbours(i, j, true).Count <= wallNeighbourMaxCount)
                    {
                        openAreaGrids.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        return openAreaGrids;
    }
}
