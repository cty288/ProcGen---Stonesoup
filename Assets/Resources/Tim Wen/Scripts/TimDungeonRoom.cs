using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PathType {
    Critical=-1,
    Normal
}
public class TimDungeonRoom : Room {
    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private GameObject buttonPrefab;

    protected int[,] roomGrids = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    [SerializeField] [Range(1, 20)] protected float dungeonRandomness;
    [SerializeField] [Range(1, 50)] protected float dungeonSpawnRandomness;
    [SerializeField] private float maxFill = 70;

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject portalGunPrefab;
    [SerializeField] protected GameObject enemySpawnerPrefab;


    protected TimRoomManager roomManager;

    protected virtual float DungeonRandomness {
         get {
            return dungeonRandomness;
        }
    }

    protected virtual float DungeonSpawnRandomness
    {
        get {
            return dungeonSpawnRandomness;
        }
    }

    [SerializeField]
    private float currentFilled = 0;

    [SerializeField] protected List<Vector2Int> exitLocations = new List<Vector2Int>();

    private LevelGenerator generator;
    private void Awake() {
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        generator = ourGenerator;
       // Debug.Log($"X: {roomGridX}, Y: {roomGridY}");
        //fill with walls
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                //Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                roomGrids[x, y] = 1;
            }
        }
        Debug.Log(roomCenter.ToString() +"    \n" +
                  $"down: {requiredExits.downExitRequired} \n" +
                  $"right: {requiredExits.rightExitRequired}\n" +
                  $"left: {requiredExits.leftExitRequired}\n" +
                  $"up: {requiredExits.upExitRequired}\n");


        ExitConstraint additionalExits = roomManager.GetAdditionalExits(new Vector2Int(roomGridX, roomGridY));
        if (additionalExits.downExitRequired) {
            requiredExits.addDirConstraint(Dir.Down);
            Debug.Log("Down additional");
        }

        if (additionalExits.leftExitRequired) {
            requiredExits.addDirConstraint(Dir.Left);
            Debug.Log("Left additional");
        }

        if (additionalExits.rightExitRequired) {
            requiredExits.addDirConstraint(Dir.Right);
            Debug.Log("Right additional");
        }

        if (additionalExits.upExitRequired) {
            requiredExits.addDirConstraint(Dir.Up);
            Debug.Log("Up additional");
        }

        exitLocations = requiredExits.requiredExitLocations().ToList();

        if (exitLocations.Count == 0 && Random.Range(0,100) >=50) {
            requiredExits.addDirConstraint((Dir) Random.Range(0, 4));
        }
        exitLocations = requiredExits.requiredExitLocations().ToList();

        if (exitLocations.Count > 0) {
            int numExitTotal = Random.Range(exitLocations.Count, 5);
            do
            {
                if (Random.Range(0, 10) >= 3)
                { //add another random exit
                    List<Dir> ignoredDir = new List<Dir>();

                    if (requiredExits.downExitRequired) ignoredDir.Add(Dir.Down);
                    if (requiredExits.leftExitRequired) ignoredDir.Add(Dir.Left);
                    if (requiredExits.rightExitRequired) ignoredDir.Add(Dir.Right);
                    if (requiredExits.upExitRequired) ignoredDir.Add(Dir.Up);

                    Dir addedExit = Dir.Right;
                    do
                    {
                        addedExit = (Dir)Random.Range(0, 4);
                    } while (ignoredDir.Contains(addedExit) && ignoredDir.Count < 4);

                    requiredExits.addDirConstraint(addedExit);
                    roomManager.SetAdditionalExitsForNeighbours(new Vector2Int(roomGridX, roomGridY), addedExit);
                }

                exitLocations = requiredExits.requiredExitLocations().ToList();
            } while (exitLocations.Count < numExitTotal);

        }




        List<Vector2Int> criticalPath = new List<Vector2Int>();
        for (int i = 0; i < exitLocations.Count - 1; i++) {
            criticalPath = GeneratePath(exitLocations[i], exitLocations[i + 1], PathType.Critical, 8);
        }

        //generate dungeons
        for (int i = 0; i < criticalPath.Count; i++) {
            if (Mathf.Abs(Vector2Int.Distance(new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2),
                criticalPath[i])) <= Random.Range(2,6)) {
                bool isDungeonStart = Random.Range(0, 100) <= DungeonSpawnRandomness;
                if (isDungeonStart) {
                    Vector2Int exitPos = Vector2Int.zero;
                    Vector2Int startPos = criticalPath[i];
                    if (startPos.y >= LevelGenerator.ROOM_HEIGHT / 2) {
                        exitPos = new Vector2Int(Random.Range(0, LevelGenerator.ROOM_WIDTH),
                            Random.Range(LevelGenerator.ROOM_HEIGHT / 2, LevelGenerator.ROOM_HEIGHT));
                    }
                    else {
                        exitPos = new Vector2Int(Random.Range(0, LevelGenerator.ROOM_WIDTH),
                            Random.Range(0, LevelGenerator.ROOM_HEIGHT / 2));
                    }

                    if(currentFilled <= maxFill) {
                        //generate critical path first
                        GeneratePath(startPos, exitPos, PathType.Normal, (int)DungeonRandomness);
                    }
                   
                }
            }
        }
       

        
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                //Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                if (roomGrids[x, y] == 1) {
                 Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }

            }
        }
        SpawnEnemySpawner();
        SpawnPortals();
        SpawnButton();
        SpawnEnemies();
        SpawnRat();
        SpawnGun();
      
    }

    [SerializeField] protected GameObject ratPrefab;
    protected void SpawnRat() {
        List<Vector2Int> openAreas = GetOpenAreas(3);
        if (openAreas.Count > 0 && Random.Range(0, 100) <= 30)
        {
            Vector2Int openArea = GlobalFuncs.randElem(openAreas);
            Tile.spawnTile(ratPrefab, transform, openArea.x, openArea.y);
        }
    }

    private void SpawnEnemySpawner() {
        List<Vector2Int> openAreas = GetOpenAreas(2);
        if (openAreas.Count > 0 && Random.Range(0, 100) <= 30)
        {
            Vector2Int openArea = GlobalFuncs.randElem(openAreas);
            Tile.spawnTile(enemySpawnerPrefab, transform, openArea.x, openArea.y);
            roomGrids[openArea.x, openArea.y] = 3;
        }
    }

    private void SpawnGun() {
        List<Vector2Int> gunSpawnPositions = GetOpenAreas(2);
        if (gunSpawnPositions.Count > 0) {
            if (Random.Range(0, 100) <= 40) {
                Vector2Int spawnPos = GlobalFuncs.randElem(gunSpawnPositions);
                Tile.spawnTile(portalGunPrefab, transform.parent, spawnPos.x, spawnPos.y);
            }
          
        }
    }

    [SerializeField] protected List<GameObject> enemiesPrefabs;
    private void SpawnEnemies() {
        List<Vector2Int> openAreas = GetOpenAreas(2);
        if (openAreas.Count > 0 && Random.Range(0,100) <= 50) {
            Vector2Int openArea = GlobalFuncs.randElem(openAreas);
            Tile.spawnTile(GlobalFuncs.randElem(enemiesPrefabs), transform, openArea.x, openArea.y);
        }
    }

    private List<Vector2Int> GetOpenAreas(int wallNeighbourMaxCount = 4) {
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
    private void SpawnButton() {
        List<Vector2Int> openAreaGrids = new List<Vector2Int>();

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++) {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++) {
                if (roomGrids[i, j] <= 0) {
                    if (GetWallNeighbours(i, j, true).Count == 0) {
                        openAreaGrids.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        if (Random.Range(0, 100) <= 10) {
            if (openAreaGrids.Count > 0) {
                Vector2Int buttonSpawnLocation = openAreaGrids[Random.Range(0, openAreaGrids.Count)];
                Tile.spawnTile(buttonPrefab, transform, buttonSpawnLocation.x, buttonSpawnLocation.y);
                roomGrids[buttonSpawnLocation.x, buttonSpawnLocation.y] = 3;
            }
        }
    }

    private void SpawnPortals() {
        List<Vector2Int> deadEndGrids = new List<Vector2Int>();
        List<Vector2Int> openAreaGrids = new List<Vector2Int>();

       // Debug.Log(roomGrids.Length + "  " + roomGrids.LongLength);
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++) {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++) {
                if (roomGrids[i, j] <= 0) {
                    if (GetWallNeighbours(i, j, true).Count == 3)
                    {
                        deadEndGrids.Add(new Vector2Int(i, j));
                    }

                    if (GetWallNeighbours(i, j, true).Count <= 1) {
                        openAreaGrids.Add(new Vector2Int(i,j));
                    }
                }
               
            }
        }

        List<Vector2Int[]> suitablePortalLocationPairs = new List<Vector2Int[]>();

        float twoPortalMinDistance = 4;
        if (deadEndGrids.Count >= 1 && openAreaGrids.Count>=1) {
            for (int i = 0; i < deadEndGrids.Count; i++) {
                for (int j = 0; j < openAreaGrids.Count; j++) {
                    if (Mathf.Abs(Vector2Int.Distance(deadEndGrids[i], openAreaGrids[j])) >= twoPortalMinDistance) {
                        suitablePortalLocationPairs.Add(new Vector2Int[2] {deadEndGrids[i], openAreaGrids[j]});
                    }
                }
            }
        }

        if (suitablePortalLocationPairs.Count >= 1) {
            if (Random.Range(0, 10) >= 5)
            {
                Vector2Int[] portalSpawnLocations =
                    suitablePortalLocationPairs[Random.Range(0, suitablePortalLocationPairs.Count)]; 


                Portal portal1 = Tile.spawnTile(portalPrefab, transform, portalSpawnLocations[0].x, portalSpawnLocations[0].y) as Portal;
                portal1.PortalType = PortalType.Entrance;
                Portal portal2 = Tile.spawnTile(portalPrefab, transform, portalSpawnLocations[1].x, portalSpawnLocations[1].y) as Portal;
                portal2.PortalType = PortalType.Exit;
                portal1.LinkedPortal = portal2;
                portal2.LinkedPortal = portal1;

                roomGrids[portalSpawnLocations[0].x, portalSpawnLocations[0].y] = 2;
                roomGrids[portalSpawnLocations[1].x, portalSpawnLocations[1].y] = 2;

            }
        }
        
    }

    private List<Vector2Int> GeneratePath(Vector2Int exitLocationStart, Vector2Int exitLocationEnd, PathType pathType, int randomness) {
        Vector2Int curPos = exitLocationStart;
        bool reachEdge = false;
        if (pathType == PathType.Critical) {
            roomGrids[curPos.x, curPos.y] = (int)pathType;
        }
      

        List<Vector2Int> retResult = new List<Vector2Int>();
        retResult.Add(curPos);

        do {
            List<Vector2Int> neighbours = GetWallNeighbours(curPos.x, curPos.y);
            Vector2Int nextPos;

            if (neighbours.Count > 0) {
                bool isFindCloestPath = Random.Range(0, 20) >= randomness - 1;

                if (isFindCloestPath) {
                    nextPos = FindCloest(neighbours, exitLocationEnd);
                }
                else {
                    nextPos = neighbours[Random.Range(0, neighbours.Count)];
                }

            }
            else {
                do {
                    if (Random.Range(0, 3) <= 0) {
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
                    else {
                        int direction = Random.Range(0, 2) == 0 ? 1 : -1;
                        nextPos = curPos + (Random.Range(0, 2) == 0
                            ? new Vector2Int(direction, 0)
                            : new Vector2Int(0, direction));
                    }
                } while (nextPos.x < 0 || nextPos.x >= LevelGenerator.ROOM_WIDTH || nextPos.y < 0 ||
                         nextPos.y >= LevelGenerator.ROOM_HEIGHT);
            }

            curPos = nextPos;
            if (pathType == PathType.Normal && (PathType) roomGrids[curPos.x, curPos.y] == PathType.Critical) {
                if (Random.Range(0, 2) == 0) {
                    break;
                }
                //break;
            }

            roomGrids[curPos.x, curPos.y] = (int)pathType;
            currentFilled++;
            if (curPos == exitLocationEnd) {
                reachEdge = true;
            }

            retResult.Add(curPos);

        } while (!reachEdge && currentFilled <= maxFill);

        return retResult;
    }

    public Vector2Int FindCloest(List<Vector2Int> points, Vector2Int targetPos) {
        float cloestDistance = 100000;
        Vector2Int cloestPoint = new Vector2Int();
        for (int i = 0; i < points.Count; i++) {
            if (Mathf.Abs(Vector2Int.Distance(points[i], targetPos)) < cloestDistance) {
                cloestDistance = Mathf.Abs(Vector2Int.Distance(points[i], targetPos));
                cloestPoint = points[i];
            }
        }

        return cloestPoint;
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
               
            }else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= LevelGenerator.ROOM_WIDTH || neighbour_y >= LevelGenerator.ROOM_HEIGHT)
            {
                if (includeBorder) {
                    if ((neighbour_x < 0 && roomGridX == 0) || (neighbour_y < 0 && roomGridY == 0) ||
                        (neighbour_x >= LevelGenerator.ROOM_WIDTH && roomGridX == generator.numXRooms - 1) ||
                        (neighbour_y >= LevelGenerator.ROOM_HEIGHT && roomGridY == generator.numYRooms - 1)) {
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
}
