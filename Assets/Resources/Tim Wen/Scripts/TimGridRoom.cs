using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimGridRoom : Room {
    protected TimRoomManager roomManager;
    protected int[,] roomGrids = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    [SerializeField] private int numWallGenerate = 30;
    private LevelGenerator generator;

    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private GameObject ratPrefab;
    private void Awake() {
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
        numWallGenerate += Random.Range(-10, 11);
    }

    [SerializeField] private List<Vector2Int> criticalPath = new List<Vector2Int>(); 

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        generator = ourGenerator;
        GenerateBorder();
        ExitConstraint additionalExits = roomManager.GetAdditionalExits(new Vector2Int(roomGridX, roomGridY));
        requiredExits = CombineExits(requiredExits, additionalExits);


        List<Vector2Int> exitLocations = requiredExits.requiredExitLocations().ToList();
        if (exitLocations.Count >= 2) {
            for (int i = 0; i < exitLocations.Count - 1; i++) {
                criticalPath = GeneratePath(exitLocations[i], exitLocations[i + 1], PathType.Critical, 0);
            }
        }
       


        List<Vector2Int> exitingGenerateWalls = new List<Vector2Int>();

        for (int i = 0; i < numWallGenerate; i++) {
            bool findSuitablePlace = true;

            for (int j = 1; j < LevelGenerator.ROOM_WIDTH-1; j++) {
                for (int k = 1; k < LevelGenerator.ROOM_HEIGHT-1; k++) {
                    findSuitablePlace = true;
                    if(criticalPath.Contains(new Vector2Int(j, k))){
                        continue;
                    }
                    foreach (Vector2Int exitingGenerateWall in exitingGenerateWalls) {
                        if (Vector2Int.Distance(new Vector2Int(j, k), exitingGenerateWall) < 2 + Random.Range(-1,2)) {

                            findSuitablePlace = false;
                            break;
                        }
                    }

                    if (findSuitablePlace) {
                        exitingGenerateWalls.Add(new Vector2Int(j, k));
                        Debug.Log($"{j}, {k}");
                        break;
                    }
                }

                if (findSuitablePlace) {
                    break;
                }
            }
            
        }


       



        foreach (Vector2Int exitingGenerateWall in exitingGenerateWalls) {
            roomGrids[exitingGenerateWall.x, exitingGenerateWall.y] = 1;
        }
        List<Vector2Int> exits = requiredExits.requiredExitLocations().ToList();
        //spawn a portal
        Vector2Int openArea = GlobalFuncs.randElem(GetOpenAreas(2));

        if (exits.Count > 0)
        {
            if (exits[0] != openArea)
            {
                Portal portal1 = Tile.spawnTile(portalPrefab, transform, exits[0].x, exits[0].y) as Portal;
                portal1.PortalType = PortalType.Entrance;

                Portal portal2 = Tile.spawnTile(portalPrefab, transform, openArea.x, openArea.y) as Portal;
                portal2.PortalType = PortalType.Exit;

                portal1.LinkedPortal = portal2;
                portal2.LinkedPortal = portal1;

                roomGrids[exits[0].x, exits[0].y] = 2;
                roomGrids[openArea.x, openArea.y] = 2;
            }
        }
        openArea = GlobalFuncs.randElem(GetOpenAreas(2));
        if (Random.Range(0, 100) >= 50) {
            TimRat rat = Tile.spawnTile(ratPrefab, transform, openArea.x, openArea.y) as TimRat;
            rat.moveTimeInterval = 2;
        }
       


        SpawnWalls(requiredExits, ourGenerator);
       
    }
    private List<Vector2Int> GeneratePath(Vector2Int exitLocationStart, Vector2Int exitLocationEnd, PathType pathType, int randomness)
    {
        Vector2Int curPos = exitLocationStart;
        bool reachEdge = false;
        if (pathType == PathType.Critical)
        {
            roomGrids[curPos.x, curPos.y] = (int)pathType;
        }


        List<Vector2Int> retResult = new List<Vector2Int>();
        retResult.Add(curPos);

        do
        {
            List<Vector2Int> neighbours = GetWallNeighbours(curPos.x, curPos.y);
            Vector2Int nextPos;

            if (neighbours.Count > 0)
            {
                bool isFindCloestPath = Random.Range(0, 20) >= randomness - 1;

                if (isFindCloestPath)
                {
                    nextPos = FindCloest(neighbours, exitLocationEnd);
                }
                else
                {
                    nextPos = neighbours[Random.Range(0, neighbours.Count)];
                }

            }
            else
            {
                do
                {
                    if (Random.Range(0, 3) <= 0)
                    {
                        Vector2Int direction = exitLocationEnd - curPos;
                        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                        {
                            nextPos = curPos + new Vector2Int((int)Mathf.Sign(direction.x), 0);
                        }
                        else
                        {
                            nextPos = curPos + new Vector2Int(0, (int)Mathf.Sign(direction.y));
                        }
                    }
                    else
                    {
                        int direction = Random.Range(0, 2) == 0 ? 1 : -1;
                        nextPos = curPos + (Random.Range(0, 2) == 0
                            ? new Vector2Int(direction, 0)
                            : new Vector2Int(0, direction));
                    }
                } while (nextPos.x < 0 || nextPos.x >= LevelGenerator.ROOM_WIDTH || nextPos.y < 0 ||
                         nextPos.y >= LevelGenerator.ROOM_HEIGHT);
            }

            curPos = nextPos;
            if (pathType == PathType.Normal && (PathType)roomGrids[curPos.x, curPos.y] == PathType.Critical)
            {
                if (Random.Range(0, 2) == 0)
                {
                    break;
                }
                //break;
            }

            roomGrids[curPos.x, curPos.y] = (int)pathType;
          
            if (curPos == exitLocationEnd)
            {
                reachEdge = true;
            }

            retResult.Add(curPos);

        } while (!reachEdge);

        return retResult;
    }

    public Vector2Int FindCloest(List<Vector2Int> points, Vector2Int targetPos)
    {
        float cloestDistance = 100000;
        Vector2Int cloestPoint = new Vector2Int();
        for (int i = 0; i < points.Count; i++)
        {
            if (Mathf.Abs(Vector2Int.Distance(points[i], targetPos)) < cloestDistance)
            {
                cloestDistance = Mathf.Abs(Vector2Int.Distance(points[i], targetPos));
                cloestPoint = points[i];
            }
        }

        return cloestPoint;
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

    public List<Vector2Int> GetWallNeighbours(int x, int y, bool includeBorder = false)
    {
        int count = 0;
        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = -1; i < 2; i++)
        {
            int neighbour_x = x + i;
            int neighbour_y = y;

            if (i == 0)
            {

            }
            else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= LevelGenerator.ROOM_WIDTH || neighbour_y >= LevelGenerator.ROOM_HEIGHT)
            {
                if (includeBorder)
                {
                    if ((neighbour_x < 0 && roomGridX == 0) || (neighbour_y < 0 && roomGridY == 0) ||
                        (neighbour_x >= LevelGenerator.ROOM_WIDTH && roomGridX == generator.numXRooms - 1) ||
                        (neighbour_y >= LevelGenerator.ROOM_HEIGHT && roomGridY == generator.numYRooms - 1))
                    {
                        result.Add(new Vector2Int(neighbour_x, neighbour_y));
                    }
                }

            }
            else if (roomGrids[neighbour_x, neighbour_y] == 1)
            {
                result.Add(new Vector2Int(neighbour_x, neighbour_y));
            }
        }

        for (int i = -1; i < 2; i++)
        {
            int neighbour_x = x;
            int neighbour_y = y + i;
            if (i == 0)
            {

            }
            else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= LevelGenerator.ROOM_WIDTH || neighbour_y >= LevelGenerator.ROOM_HEIGHT)
            {

            }
            else if (roomGrids[neighbour_x, neighbour_y] == 1)
            {
                result.Add(new Vector2Int(neighbour_x, neighbour_y));
            }
        }
        return result;
    }

    private void SpawnWalls(ExitConstraint exits, LevelGenerator ourGenerator) {
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {

                if (((x== LevelGenerator.ROOM_WIDTH/2 || x == LevelGenerator.ROOM_WIDTH / 2 -1) && y ==0)) continue;
                if (((x == LevelGenerator.ROOM_WIDTH / 2 || x == LevelGenerator.ROOM_WIDTH / 2 - 1) && y == LevelGenerator.ROOM_HEIGHT-1)) continue;
                if (((y == LevelGenerator.ROOM_HEIGHT / 2 || y == LevelGenerator.ROOM_HEIGHT / 2 - 1) && x == 0)) continue;
                if ( ((y == LevelGenerator.ROOM_HEIGHT / 2 || y == LevelGenerator.ROOM_HEIGHT / 2 -1) && x == LevelGenerator.ROOM_WIDTH-1)) continue;


                if (roomGrids[x, y] == 1) {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }
    }


    private ExitConstraint CombineExits(ExitConstraint requiredExits, ExitConstraint additionalExits) {
        if (additionalExits.downExitRequired)
        {
            requiredExits.addDirConstraint(Dir.Down);
        }

        if (additionalExits.leftExitRequired)
        {
            requiredExits.addDirConstraint(Dir.Left);
        }

        if (additionalExits.rightExitRequired)
        {
            requiredExits.addDirConstraint(Dir.Right);
        }

        if (additionalExits.upExitRequired)
        {
            requiredExits.addDirConstraint(Dir.Up);
        }

        return requiredExits;
    }
    private void GenerateBorder() {
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1) {
                    roomGrids[x, y] = 1;
                }
            }
        }
    }
}
