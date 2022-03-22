using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimTeleportRoom : Room
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private List<GameObject> treasurePrefabs;
    protected int[,] roomGrids = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    protected TimRoomManager roomManager;
    [SerializeField] protected List<Vector2Int> exitLocations = new List<Vector2Int>();
    private LevelGenerator generator;
    private void Awake()
    {
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
    }

    public override Room createRoom(ExitConstraint requiredExits) {
        return base.createRoom(requiredExits);
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

        

        exitLocations = requiredExits.requiredExitLocations().ToList();
        Dir addedExit = (Dir)Random.Range(0, 4);
        requiredExits.addDirConstraint(addedExit);
        roomManager.SetAdditionalExitsForNeighbours(new Vector2Int(roomGridX, roomGridY), addedExit);

        if (requiredExits.downExitRequired) {
            roomGrids[LevelGenerator.ROOM_WIDTH/2, 0] = 0;
            roomGrids[LevelGenerator.ROOM_WIDTH / 2-1, 0] = 0;
        }

        if (requiredExits.leftExitRequired) {
            roomGrids[0, LevelGenerator.ROOM_HEIGHT/2] = 0;
            roomGrids[0, LevelGenerator.ROOM_HEIGHT / 2-1] = 0;
        }

        if (requiredExits.rightExitRequired) {
            roomGrids[LevelGenerator.ROOM_WIDTH-1, LevelGenerator.ROOM_HEIGHT / 2] = 0;
            roomGrids[LevelGenerator.ROOM_WIDTH-1, LevelGenerator.ROOM_HEIGHT / 2-1] = 0;
        }

        if (requiredExits.upExitRequired) {
            roomGrids[LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT-1] = 0;
            roomGrids[LevelGenerator.ROOM_WIDTH / 2 - 1, LevelGenerator.ROOM_HEIGHT-1] = 0;
        }
        SpawnInnerWalls();

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                //Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                if (roomGrids[x, y] == 1) {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }

        SpawnPortal();
        SpawnTreasure();
    }

    private void SpawnTreasure() {

        Tile.spawnTile(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)], transform,
            Random.Range(0,2) == 0 ? 3 : 6, Random.Range(3, 5));
    }

    private void SpawnPortal() {
        int portalPos = Random.Range(0, 2) == 0 ? 4 : 5;
        List<Portal> exitPortals = GameObject.FindObjectsOfType<Portal>().ToList();

        if (exitPortals.Count > 0) {
            Portal portal = Tile.spawnTile(portalPrefab, transform, Random.Range(4, 6), Random.Range(3, 5)) as Portal;
            portal.PortalType = PortalType.Entrance;
            Portal exitPortal = exitPortals[Random.Range(0, exitPortals.Count)];
            portal.LinkedPortal = exitPortal;
        }

    }

    private void SpawnInnerWalls() {
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                if ((x >= 2 && x <= 7 && y >= 2 && y <= 5)) {
                    roomGrids[x, y] = 1;
                }
            }


        }


        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if ((x >= 3 && x <= 6 && y >= 3 && y <= 4))
                {
                    roomGrids[x, y] = 0;
                }
            }
        }
    }
}
