using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimValidatedRoom : Room
{
    public bool hasUpExit, hasDownExit, hasLeftExit, hasRightExit;

    public bool hasLeftDownPath, hasLeftUpPath, hasLeftRightPath, hasRightDownPath, hasRightUpPath, hasUpDownPath;

    private void Awake()
    {
        //ValidateRoom();
    }

    public bool MeetsConstraints(ExitConstraint requiredExits)
    {
        ValidateRoom();
        if (requiredExits.upExitRequired && hasUpExit == false)
        {
            return false;
        }
        if (requiredExits.downExitRequired && hasDownExit == false)
        {
            return false;
        }
        if (requiredExits.leftExitRequired && hasLeftExit == false)
        {
            return false;
        }
        if (requiredExits.rightExitRequired && hasRightExit == false)
        {
            return false;
        }

        if (hasLeftDownPath == false && requiredExits.leftExitRequired && requiredExits.downExitRequired)
        {
            return false;
        }
        if (hasLeftUpPath == false && requiredExits.leftExitRequired && requiredExits.upExitRequired)
        {
            return false;
        }
        if (hasLeftRightPath == false && requiredExits.leftExitRequired && requiredExits.rightExitRequired)
        {
            return false;
        }
        if (hasRightDownPath == false && requiredExits.rightExitRequired && requiredExits.downExitRequired)
        {
            return false;
        }
        if (hasRightUpPath == false && requiredExits.rightExitRequired && requiredExits.upExitRequired)
        {
            return false;
        }
        if (hasUpDownPath == false && requiredExits.upExitRequired && requiredExits.downExitRequired)
        {
            return false;
        }
        return true;

    }

    public bool DoesPathExist(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();

        if (IsPointNavigable(indexGrid, startPoint))
        {
            openSet.Add(startPoint);
        }

        while (openSet.Count > 0)
        {
            Vector2Int currentPoint = openSet[0];
            openSet.RemoveAt(0);

            if (currentPoint == endPoint)
            {
                return true;
            }
            Vector2Int upNeighbour = new Vector2Int(currentPoint.x, currentPoint.y + 1);
            if (IsPointInGrid(upNeighbour) && openSet.Contains(upNeighbour) == false && closedSet.Contains(upNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, upNeighbour))
                {
                    openSet.Add(upNeighbour);
                }
            }
            Vector2Int downNeighbour = new Vector2Int(currentPoint.x, currentPoint.y - 1);
            if (IsPointInGrid(downNeighbour) && openSet.Contains(downNeighbour) == false && closedSet.Contains(downNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, downNeighbour))
                {
                    openSet.Add(downNeighbour);
                }
            }
            Vector2Int rightNeighbour = new Vector2Int(currentPoint.x + 1, currentPoint.y);
            if (IsPointInGrid(rightNeighbour) && openSet.Contains(rightNeighbour) == false && closedSet.Contains(rightNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, rightNeighbour))
                {
                    openSet.Add(rightNeighbour);
                }
            }
            Vector2Int leftNeighbour = new Vector2Int(currentPoint.x - 1, currentPoint.y);
            if (IsPointInGrid(leftNeighbour) && openSet.Contains(leftNeighbour) == false && closedSet.Contains(leftNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, leftNeighbour))
                {
                    openSet.Add(leftNeighbour);
                }
            }

            closedSet.Add(currentPoint);
        }

        return false;
    }

    bool IsPointInGrid(Vector2 point)
    {
        if (point.x < 0 || point.x >= LevelGenerator.ROOM_WIDTH || point.y < 0 || point.y >= LevelGenerator.ROOM_HEIGHT)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool IsPointNavigable(int[,] indexGrid, Vector2Int point)
    {
        if (IsPointInGrid(point) == false)
        {
            return false;
        }

        if (indexGrid[point.x, point.y] == 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int[,] LoadIndexGrid()
    {
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
        return indexGrid;
    }



    private void ValidateRoom()
    {
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);

        int[,] indexGrid = LoadIndexGrid();

        hasLeftExit = IsPointNavigable(indexGrid, leftExit);
        hasRightExit = IsPointNavigable(indexGrid, rightExit);
        hasUpExit = IsPointNavigable(indexGrid, upExit);
        hasDownExit = IsPointNavigable(indexGrid, downExit);


        hasUpDownPath = DoesPathExist(indexGrid, upExit, downExit);
        hasLeftUpPath = DoesPathExist(indexGrid, upExit, leftExit);
        hasRightUpPath = DoesPathExist(indexGrid, upExit, rightExit);
        hasLeftDownPath = DoesPathExist(indexGrid, downExit, leftExit);
        hasRightDownPath = DoesPathExist(indexGrid, downExit, rightExit);
        hasLeftRightPath = DoesPathExist(indexGrid, leftExit, rightExit);

    }
}
