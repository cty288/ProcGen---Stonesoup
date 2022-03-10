using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SearchVertex
{

    public Vector2Int point;
    public SearchVertex parent;

    public SearchVertex(Vector2Int point, SearchVertex parent)
    {
        this.point = point;
        this.parent = parent;
    }
}

public class WenValidateRoom : Room
{
    public bool hasUpExit, hasDownExit, hasLeftExit, hasRightExit;

    public bool hasLeftDownPath, hasLeftUpPath, hasLeftRightPath,hasRightDownPath, hasRightUpPath, hasUpDownPath;

    private void Awake()
    {
        ValidateRoom();
    }

    public bool MeetsConstraints(ExitConstraint requiredExits)
    {
        if(requiredExits.upExitRequired && hasUpExit == false)
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

    public bool DoesListContainPoint(List<SearchVertex> list, Vector2Int point)
    {
        foreach(SearchVertex sv in list)
        {
            if(sv.point == point)
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector2Int> GetPathDFS(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint)
    {
        List<SearchVertex> openSet = new List<SearchVertex>();
        List<SearchVertex> closedSet = new List<SearchVertex>();
        //List<Vector2Int> path = new List<Vector2Int>();
        if (IsPointNavigable(indexGrid, startPoint))
        {
            openSet.Add(new SearchVertex(startPoint, null));
        }

        while (openSet.Count > 0)
        {
            int index = openSet.Count - 1;
            //Vector2Int currentPoint = openSet[0];
            SearchVertex currentVertex = openSet[index];
            //openSet.RemoveAt(0);
            openSet.RemoveAt(index);
            if (currentVertex.point == endPoint)
            {
                List<Vector2Int> retVal = new List<Vector2Int>();
                while(currentVertex != null)
                {
                    retVal.Add(currentVertex.point);
                    currentVertex = currentVertex.parent;
                }
                retVal.Reverse();

                return retVal;
            }
            Vector2Int upNeighbour = new Vector2Int(currentVertex.point.x, currentVertex.point.y + 1);
            
            if (IsPointInGrid(upNeighbour) && DoesListContainPoint(openSet, upNeighbour) == false && DoesListContainPoint(closedSet, upNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, upNeighbour))
                {
                    SearchVertex upNeighbourVertex = new SearchVertex(upNeighbour, currentVertex);
                    openSet.Add(upNeighbourVertex);
                }
            }
            Vector2Int downNeighbour = new Vector2Int(currentVertex.point.x, currentVertex.point.y - 1);
            if (IsPointInGrid(downNeighbour) && DoesListContainPoint(openSet, downNeighbour) == false && DoesListContainPoint(closedSet, downNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, downNeighbour))
                {
                    SearchVertex downNeighbourVertex = new SearchVertex(downNeighbour, currentVertex);
                    openSet.Add(downNeighbourVertex);
                }
            }
            Vector2Int rightNeighbour = new Vector2Int(currentVertex.point.x + 1, currentVertex.point.y);
            if (IsPointInGrid(rightNeighbour) && DoesListContainPoint(openSet, rightNeighbour) == false && DoesListContainPoint(closedSet, rightNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, rightNeighbour))
                {
                    SearchVertex rightNeighbourVertex = new SearchVertex(rightNeighbour, currentVertex);
                    openSet.Add(rightNeighbourVertex);
                }
            }
            Vector2Int leftNeighbour = new Vector2Int(currentVertex.point.x - 1, currentVertex.point.y);
            if (IsPointInGrid(leftNeighbour) && DoesListContainPoint(openSet, leftNeighbour) == false && DoesListContainPoint(closedSet, leftNeighbour) == false)
            {
                if (IsPointNavigable(indexGrid, leftNeighbour))
                {
                    SearchVertex leftNeighbourVertex = new SearchVertex(leftNeighbour, currentVertex);
                    openSet.Add(leftNeighbourVertex);
                }
            }

            closedSet.Add(currentVertex);
        }

        return new List<Vector2Int>();
    }

    bool IsPointInGrid(Vector2 point)
    {
        if (point.x < 0 || point.x >= LevelGenerator.ROOM_WIDTH || point.y < 0 || point.y >= LevelGenerator.ROOM_HEIGHT)
        {
            return false;
        }else
        {
            return true;
        }
    }

    bool IsPointNavigable(int[,] indexGrid, Vector2Int point)
    {
        if(IsPointInGrid(point) == false)
        {
            return false;
        }

        if(indexGrid[point.x,point.y] == 1)
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


        hasUpDownPath = GetPathDFS(indexGrid, upExit, downExit).Count > 0;
        hasLeftUpPath = GetPathDFS(indexGrid, upExit, leftExit).Count > 0;
        hasRightUpPath = GetPathDFS(indexGrid, upExit, rightExit).Count > 0;
        hasLeftDownPath = GetPathDFS(indexGrid, downExit, leftExit).Count > 0;
        hasRightDownPath = GetPathDFS(indexGrid, downExit, rightExit).Count > 0;
        hasLeftRightPath = GetPathDFS(indexGrid, leftExit, rightExit).Count > 0;

        if (hasUpDownPath)
        {
            List<Vector2Int> path = GetPathDFS(indexGrid, upExit, leftExit);
            foreach(Vector2Int point in path)
            {
                Debug.Log(point);
            }
        }

    }



}
