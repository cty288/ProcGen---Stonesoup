using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WenRoom : Room
{
	public enum RoomContentType
    {
		normal = 0,
		trap = 1,
		rest = 2,
		reward = 3,
		secret = 4
    }

	public static int secretNum = 1;

    public List<GameObject> trapList;
	public List<GameObject> mechanismList;
    public List<GameObject> enemyList;
	public List<GameObject> restSupplyList;
	public List<GameObject> rewardList;

	public RoomContentType roomContentType;

	public float borderWallProbability = 0.3f;
    protected TimRoomManager roomManager;

    [SerializeField] private List<Room> designedRooms;
    public override Room createRoom(ExitConstraint requiredExits) {
        if (Random.Range(0, 100) >= 70) {
            return base.createRoom(requiredExits);
		}
		//Debug.Log("Designed room");
        Room createdRoom = GlobalFuncs.randElem(designedRooms).createRoom(requiredExits);
        return createdRoom;
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        if (!GameObject.Find("_GameManager").GetComponent<TimRoomManager>())
        {
            GameObject.Find("_GameManager").AddComponent<TimRoomManager>();
        }
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
		

        ExitConstraint additionalExits = roomManager.GetAdditionalExits(new Vector2Int(roomGridX, roomGridY));
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

		roomContentType = GetRoomContentType();
		if(roomContentType == RoomContentType.normal)
        {
			GenerateNormalRoom(ourGenerator, requiredExits);
        }
        else if(roomContentType == RoomContentType.trap)
        {
			GenerateTrapRoom(ourGenerator, requiredExits);
		}
		else if (roomContentType == RoomContentType.rest)
		{
			GenerateRestRoom(ourGenerator, requiredExits);
		}
		else if (roomContentType == RoomContentType.reward)
		{
			GenerateRewardRoom(ourGenerator, requiredExits);
		}
	}

    public GameObject GetRandomSeries(List<GameObject> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

	public RoomContentType GetRoomContentType()
	{
		int randomIndex = Random.Range(0, 63);
		RoomContentType randomType = RoomContentType.normal;
        if (randomIndex < 25)
        {
			randomType = RoomContentType.normal;
        }
		else if(randomIndex >= 25 && randomIndex < 50)
        {
			randomType = RoomContentType.trap;
		}
		else if (randomIndex >= 50 && randomIndex < 55)
		{
			randomType = RoomContentType.rest;
		}
		else if (randomIndex >= 55 && randomIndex < 60)
		{
            randomType = RoomContentType.normal;
			//randomType = RoomContentType.normal;
		}
		/*else if (randomIndex >= 60 && randomIndex < 62 && secretNum > 0)
		{
			secretNum--;
			randomType = RoomContentType.secret;
        }*/
        else
        {
			randomType = RoomContentType.normal;
		}

		return randomType;
	}

	protected void GenerateTrapRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		// In this version of room generation, I generate walls and then other stuff.
		generateBorders(ourGenerator, requiredExits);

		//int numTraps = Random.Range(6, 15);
		int numArrows = Random.Range(2, 5);

		bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
		for (int x = 1; x < LevelGenerator.ROOM_WIDTH - 1; x++)
		{
			for (int y = 1; y < LevelGenerator.ROOM_HEIGHT - 1; y++)
			{
				if ((y == LevelGenerator.ROOM_HEIGHT / 2 || y == LevelGenerator.ROOM_HEIGHT / 2 - 1) || (x == LevelGenerator.ROOM_WIDTH / 2 || x == LevelGenerator.ROOM_WIDTH / 2 - 1))
				{
					occupiedPositions[x, y] = false;
					if (Random.value <= 0.22f)
					{
						Tile.spawnTile(trapList[0], transform, x, y);
					}
				}
				else
				{
					occupiedPositions[x, y] = true;
				}
			}
		}

		for (int i = 0; i < numArrows; i++)
		{
			GameObject arrowMech;

			int x = Random.Range(1, LevelGenerator.ROOM_WIDTH - 1);
			int y = Random.Range(1, LevelGenerator.ROOM_HEIGHT - 1);

			if (!occupiedPositions[x, y] || Mathf.Abs(x - LevelGenerator.ROOM_WIDTH) < 2 || Mathf.Abs(x - LevelGenerator.ROOM_WIDTH + 1) < 2
				|| Mathf.Abs(y - LevelGenerator.ROOM_HEIGHT) < 2 || Mathf.Abs(y - LevelGenerator.ROOM_HEIGHT + 1) < 2)
			{
				i--;
				continue;
			}

			if (occupiedPositions[x, y] && x < LevelGenerator.ROOM_WIDTH / 2 - 1)
			{
				arrowMech = mechanismList[0];
				for (int nx = x; nx < LevelGenerator.ROOM_WIDTH / 2 - 1; nx++)
				{
					occupiedPositions[nx, y] = false;
				}
				Tile.spawnTile(arrowMech, transform, x, y);
			}
			else if (occupiedPositions[x, y] && x > LevelGenerator.ROOM_WIDTH / 2)
			{
				arrowMech = mechanismList[1];
				for (int nx = x; nx > LevelGenerator.ROOM_WIDTH / 2; nx--)
				{
					occupiedPositions[nx, y] = false;
				}
				Tile.spawnTile(arrowMech, transform, x, y);
			}
		}
		for (int x = 1; x < LevelGenerator.ROOM_WIDTH - 1; x++)
		{
			for (int y = 1; y < LevelGenerator.ROOM_HEIGHT - 1; y++)
			{
                if (occupiedPositions[x, y])
                {
					Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
			}
		}
	}


	protected void GenerateNormalRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		// In this version of room generation, I generate walls and then other stuff.
		generateBorders(ourGenerator, requiredExits);
		// Inside the borders I make some rocks and enemies.
		int numTraps = Random.Range(0,  10);
		int numEnemies = (10 - numTraps) / 5;

		// First, let's make an array keeping track of where we've spawned objects already.
		bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
		{
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
			{
				if ((x >= 1 && x <= LevelGenerator.ROOM_WIDTH - 2) && y >= 1 && y <= LevelGenerator.ROOM_HEIGHT - 2){

					float borderRoom = Mathf.Abs((Mathf.Abs(x - LevelGenerator.ROOM_WIDTH/2) + Mathf.Abs(y - LevelGenerator.ROOM_HEIGHT / 2)) * 0.1f);
					if (Random.value <= borderRoom)
                    {
                        occupiedPositions[x, y] = true;
                        Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    }
				}
				
				/*
				if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
				{
					// All border zones are occupied.
					occupiedPositions[x, y] = true;
				}
				else if (x == 1 || x == LevelGenerator.ROOM_WIDTH - 2 || y == 1 || y == LevelGenerator.ROOM_HEIGHT - 2)
				{
					// All border zones (in 1 grid) are occupied.
					occupiedPositions[x, y] = true;
				}
				else
				{
                    if (Random.value <= 0.2f)
                    {
						occupiedPositions[x, y] = true;
						Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    }
                    else
                    {
						occupiedPositions[x, y] = false;
					}
				}*/
			}
		}

		// Now we spawn rocks and enemies in random locations
		List<Vector2> possibleSpawnPositions = new List<Vector2>(LevelGenerator.ROOM_WIDTH * LevelGenerator.ROOM_HEIGHT);
		GameObject trapPrefab = GetRandomSeries(trapList);
		for (int i = 0; i < numTraps; i++)
		{
			possibleSpawnPositions.Clear();
			for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
			{
				for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
				{
					if (occupiedPositions[x, y])
					{
						continue;
					}
					possibleSpawnPositions.Add(new Vector2(x, y));
				}
			}
			if (possibleSpawnPositions.Count > 0)
			{
				Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
				Tile.spawnTile(trapPrefab, transform, (int)spawnPos.x, (int)spawnPos.y);
				occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
			}
		}
		GameObject enemyPrefab = GetRandomSeries(enemyList);
		for (int i = 0; i < numEnemies; i++)
		{
			possibleSpawnPositions.Clear();
			for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
			{
				for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
				{
					if (occupiedPositions[x, y])
					{
						continue;
					}
					possibleSpawnPositions.Add(new Vector2(x, y));
				}
			}
			if (possibleSpawnPositions.Count > 0)
			{
				Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
				Tile.spawnTile(enemyPrefab, transform, (int)spawnPos.x, (int)spawnPos.y);
				occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
			}
		}
	}

	protected void GenerateRestRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		generateBorders(ourGenerator, requiredExits);

		int supplyNum = Random.Range(1, 4);

		bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
		{
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
				if ((x >= 1 && x <= LevelGenerator.ROOM_WIDTH - 2) && y >= 1 && y <= LevelGenerator.ROOM_HEIGHT - 2)
                {
					float borderRoom = Mathf.Abs(1 - x * y * 0.2f);
					if (Random.value <= borderRoom)
                    {
                        occupiedPositions[x, y] = true;
                        Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    }
                }

				/*
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
				{
					// All border zones are occupied.
					occupiedPositions[x, y] = true;
				}
				else if (x == 1 || x == LevelGenerator.ROOM_WIDTH - 2 || y == 1 || y == LevelGenerator.ROOM_HEIGHT - 2)
				{
					// All border zones (in 1 grid) are occupied.
                    if (Random.value <= 0.5) {
                        occupiedPositions[x, y] = true;
					}
						
				}
				else
				{
					if (Random.value <= 0.4f)
					{
						occupiedPositions[x, y] = true;
						Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
					}
					else
					{
						occupiedPositions[x, y] = false;
					}
				}*/
			}
		}

		for(int i = 0; i < supplyNum; i++)
        {
			int x = Random.Range(1, LevelGenerator.ROOM_WIDTH - 1);
			int y = Random.Range(1, LevelGenerator.ROOM_HEIGHT - 1);

			if (!occupiedPositions[x, y])
			{
				i--;
				continue;
			}

		//	Debug.Log(restSupplyList[0].name);
			Tile.spawnTile(restSupplyList[0], transform, x, y);
		}

	}

	protected void GenerateRewardRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		generateBorders(ourGenerator, requiredExits);

		GameObject reward = GetRandomSeries(rewardList);

		Tile.spawnTile(reward, transform, LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT / 2);
	}

	protected void generateBorders(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		// Basically we go over the border and determining where to spawn walls.
		bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
		{
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
			{
				if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
				{
					if ((x == LevelGenerator.ROOM_WIDTH / 2|| x == LevelGenerator.ROOM_WIDTH / 2 - 1) && y == LevelGenerator.ROOM_HEIGHT - 1)
					{
						if (roomContentType != RoomContentType.secret)
						{
							wallMap[x, y] = false;
                        }
                        else
                        {
							wallMap[x, y] = true;
						}
					}
					else if (x == LevelGenerator.ROOM_WIDTH - 1 && (y == LevelGenerator.ROOM_HEIGHT / 2 || y == LevelGenerator.ROOM_HEIGHT/2 - 1))
					{
						if (roomContentType != RoomContentType.secret)
						{
							wallMap[x, y] = false;
						}
						else
						{
							wallMap[x, y] = true;
						}
					}
					else if ((x == LevelGenerator.ROOM_WIDTH / 2 || x == LevelGenerator.ROOM_WIDTH / 2 - 1) && y == 0)
					{
						if (roomContentType != RoomContentType.secret)
						{
							wallMap[x, y] = false;
						}
						else
						{
							wallMap[x, y] = true;
						}
					}
					else if (x == 0 && (y == LevelGenerator.ROOM_HEIGHT / 2 || y == LevelGenerator.ROOM_HEIGHT / 2 - 1))
					{
						if (roomContentType != RoomContentType.secret)
						{
							wallMap[x, y] = false;
						}
						else
						{
							wallMap[x, y] = true;
						}
					}
					else
					{
						wallMap[x, y] = true;
					}
					continue;
				}
				wallMap[x, y] = false;
			}
		}

		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
		{
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
			{
				if (wallMap[x, y])
				{
					Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
				}
			}
		}
	}

}
