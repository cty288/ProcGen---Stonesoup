using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
