using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimRoom : Room {
    public List<Tile> allTiles;
    [SerializeField] private GameObject portalPrefab;

    private void Awake() {
        allTiles = new List<Tile>();
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
		string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++)
        {
            string row = rows[height - r - 1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++)
            {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int tileIndex = indexGrid[i, j];
                if (tileIndex == 0)
                {
                    continue; // 0 is nothing.
                }
                GameObject tileToSpawn;
                if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                {
                    tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                }
                else
                {
                    tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
                }
                Tile tile = Tile.spawnTile(tileToSpawn, transform, i, j);
                allTiles.Add(tile);

            }
        }

        List<Tile> addedPortals = new List<Tile>();

        foreach (Tile tile in allTiles) {
            //portal random generation
            if (tile.GetType() == typeof(Portal))
            {
                Vector2Int spawnGridPos;
                bool overlap = false;
                do {
                    int spawnMinX = 1;
                    int spawnMaxX = width - 1;
                    int spawnMinY = 1;
                    int spawnMaxY = height - 1;

                    spawnGridPos = new Vector2Int(Random.Range(spawnMinX, spawnMaxX ),
                        Random.Range(spawnMinY, spawnMaxY));
                    Debug.Log(spawnGridPos);
                    overlap = false;
                    if (indexGrid[spawnGridPos.x, spawnGridPos.y] != 0) {
                        overlap = true;
                    }
                } while (Vector2.Distance(spawnGridPos, Tile.toGridCoord(tile.transform.position)) <= 6 || overlap);
                

                Portal connectedPortal = Tile.spawnTile(portalPrefab, transform, spawnGridPos.x, spawnGridPos.y) as Portal;
                connectedPortal.LinkedPortal = tile as Portal;
                connectedPortal.PortalType = PortalType.Exit;
                (tile as Portal).LinkedPortal = connectedPortal;
                addedPortals.Add(connectedPortal);
            }
        }

        allTiles.AddRange(addedPortals);

    }
}
