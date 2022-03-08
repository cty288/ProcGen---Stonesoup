using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileExtension {
    /// <summary>
    /// A temp solution to spawn the missiles
    /// </summary>
    /// <param name="t"></param>
    /// <param name="tilePrefab"></param>
    /// <param name="parentOfTile"></param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    public static Tile SpawnTile(this Tile t, GameObject tilePrefab, Transform parentOfTile, int gridX, int gridY) {
       
        GameObject tileObj = GameObject.Instantiate(tilePrefab) as GameObject;
        tileObj.transform.parent = parentOfTile;
        Tile tile = tileObj.GetComponent<Tile>();
        Vector2 tilePos =Tile.toWorldCoord(gridX, gridY);
        tile.localX = tilePos.x;
        tile.localY = tilePos.y;
        tile.init();
        return tile;
    }
}


public class AirStrikeButton : Tile {
    private Animator animator;
    [SerializeField] private GameObject misslePrefab;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public override void useAsItem(Tile tileUsingUs) {
        base.useAsItem(tileUsingUs);
        animator.SetTrigger("OnPressed");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = toGridCoord(mousePosition) + new Vector2(0, 30);
        
        Debug.Log(mousePosition);
        Debug.Log(gridPos);
        Tile missle = this.SpawnTile(misslePrefab, null, (int) gridPos.x,  (int) gridPos.y);
        ((Missile) missle).TargetGridPos = toGridCoord(mousePosition);
        StartCoroutine(SelfDestroy());
    }


    IEnumerator SelfDestroy() {
        yield return new WaitForSeconds(0.3f);
        die();
    }
}
