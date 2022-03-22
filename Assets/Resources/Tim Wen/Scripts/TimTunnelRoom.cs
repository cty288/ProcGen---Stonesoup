using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimTunnelRoom : Room
{
    protected TimRoomManager roomManager;
    private LevelGenerator generator;
    protected int[,] roomGrids = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    [SerializeField] protected List<Vector2Int> exitLocations = new List<Vector2Int>();
    [SerializeField] [Range(0, 20)] private int randomness;
    [SerializeField] private List<GameObject> treasurePrefabs;
    [SerializeField] private Vector2Int entrance;
    private void Awake()
    {
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


        generator = ourGenerator;
        ExitConstraint additionalExits = roomManager.GetAdditionalExits(new Vector2Int(roomGridX, roomGridY));
       //Debug.Log( "TunnelRoom" + new Vector2Int(roomGridX, roomGridY));
        requiredExits = SetupRequiredExits(requiredExits, additionalExits);
        int exitCount = Random.Range(0, 4);

        

        if (requiredExits.requiredExitLocations().ToList().Count > 0|| exitCount>0) {
            //always contain at least 1 element
            if (requiredExits.requiredExitLocations().ToList().Count == 0) {
                requiredExits.addDirConstraint((Dir) Random.Range(0, 4));
            }
            
            List<Vector2Int> entrances = requiredExits.requiredExitLocations().ToList();
           // entrances.AddRange(roomManager.GetAdditionalExits(new Vector2Int(roomGridX, roomGridY)).requiredExitLocations().ToList());
            
            entrance = entrances.First();

          

            if (exitCount == 0) { //dead end -> generate treasure
                Vector2Int exitLocationEnd = new Vector2Int(Random.Range(0, LevelGenerator.ROOM_WIDTH - 1),
                    Random.Range(0, LevelGenerator.ROOM_HEIGHT - 1));
                GeneratePath(entrance,exitLocationEnd , PathType.Critical, randomness);
                Tile.spawnTile(GlobalFuncs.randElem(treasurePrefabs), transform, exitLocationEnd.x, exitLocationEnd.y);
            }
            else
            {
                do
                {

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

                    exitLocations = requiredExits.requiredExitLocations().ToList();
                } while (exitLocations.Count < exitCount);


                for (int i = 0; i < exitLocations.Count-1; i++)
                {
                    GeneratePath(exitLocations[i], exitLocations[i+1], PathType.Critical, randomness);
                }
            }
        }
       
       

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
                //Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                if (roomGrids[x, y] == 1) {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }

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
                    if (exitLocationStart.x == exitLocationEnd.x || exitLocationStart.y == exitLocationEnd.y) {
                        nextPos = FindCloest(neighbours, exitLocationEnd);
                    }
                    else {
                        Vector2Int direction = exitLocationEnd - curPos;

                        if (exitLocationStart.x == 0 || exitLocationStart.x == LevelGenerator.ROOM_WIDTH - 1) {
                            if (curPos.x != exitLocationEnd.x) {
                                nextPos = curPos + new Vector2Int((int)Mathf.Sign(direction.x), 0);
                            }
                            else {
                                nextPos = FindCloest(neighbours, exitLocationEnd);
                            }
                        }
                        else {
                            if (curPos.y != exitLocationEnd.y)
                            {
                                nextPos = curPos + new Vector2Int(0, (int)Mathf.Sign(direction.y));
                            }
                            else
                            {
                                nextPos = FindCloest(neighbours, exitLocationEnd);
                            }
                        }
                    }
                    
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
          

            roomGrids[curPos.x, curPos.y] = (int)pathType;
            if (curPos == exitLocationEnd) {
                reachEdge = true;
            }

            retResult.Add(curPos);

        } while (!reachEdge);

        return retResult;
    }


    private ExitConstraint SetupRequiredExits(ExitConstraint requiredExits, ExitConstraint additionalExits)
    {
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
}
