using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileExtension
{
    /// <summary>
    /// A temp solution to spawn the missiles
    /// </summary>
    /// <param name="t"></param>
    /// <param name="tilePrefab"></param>
    /// <param name="parentOfTile"></param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    public static Tile SpawnTile(this Tile t, GameObject tilePrefab, Transform parentOfTile, int gridX, int gridY)
    {

        GameObject tileObj = GameObject.Instantiate(tilePrefab) as GameObject;
        tileObj.transform.parent = parentOfTile;
        Tile tile = tileObj.GetComponent<Tile>();
        Vector2 tilePos = Tile.toWorldCoord(gridX, gridY);
        tile.localX = tilePos.x;
        tile.localY = tilePos.y;
        tile.init();
        return tile;
    }

    public static void AddMaximumHealth(this Tile tile, int value)
    {
        int health = (int)tile.GetType().GetField("_startHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(tile);

        tile.GetType().GetField("_startHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tile, health + value);
        tile.health += value;
    }
}
