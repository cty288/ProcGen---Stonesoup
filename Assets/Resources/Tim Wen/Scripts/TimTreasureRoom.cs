using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimTreasureRoom : Room
{
    protected int[,] roomGrids = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    private TimRoomManager roomManager;
    private LevelGenerator generator;
    [SerializeField]
    private float wallToCenterDistance = 4;

    [SerializeField] private List<GameObject> treasurePrefabs;
    [SerializeField] private List<GameObject> enemyPrefabs;
    private void Awake()
    {
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
    }
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        generator = ourGenerator;
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1) {
                    roomGrids[x, y] = 1;
                }
            }
        }

        SpawnInnerWalls();
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                //Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                if (roomGrids[x, y] == 1) {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }

        //spawn random treasures
        Tile.spawnTile(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)], transform,
            LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2);

        SpawnRandomEnemy();
    }

    private void SpawnRandomEnemy() {
        List<Vector2Int> availableGrids = new List<Vector2Int>();

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++) {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++) {
                if (roomGrids[i, j] == 0 && i!=LevelGenerator.ROOM_WIDTH/2 && j!= LevelGenerator.ROOM_HEIGHT/2) {
                    availableGrids.Add(new Vector2Int(i,j));
                }
            }
        }

        if (availableGrids.Count > 0) {
            Vector2Int spawnPos = availableGrids[Random.Range(0, availableGrids.Count)];
            GameObject spawnPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Tile.spawnTile(spawnPrefab, transform, spawnPos.x, spawnPos.y);
        }
     
    }

    private void SpawnInnerWalls() {
        Vector2Int center = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2);
        for (int x = 1; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 1; y < LevelGenerator.ROOM_HEIGHT; y++) {
                float distanceToCenter =Mathf.Abs( Vector2Int.Distance(new Vector2Int(x, y), center));
                float spawnChance = Mathf.Min(distanceToCenter / Random.Range(wallToCenterDistance-1f, wallToCenterDistance +1f), 1);
               // Debug.Log(spawnChance);
                if (Random.Range(0f, 1f) <= Mathf.Pow(spawnChance,5)) {
                    roomGrids[x, y] = 1;
                }

               // if (spawnChance == 1) {
                   // roomGrids[x, y] = 1;
                //}
            }
        }
    }
}
