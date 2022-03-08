using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Tile {
    public Vector2 TargetGridPos;
    private Rigidbody2D rigidbody;
    [SerializeField] private float force = 20;
    [SerializeField] private float explodeRadius = 2;
    [SerializeField] private float explodeForce = 10;
    private bool reached = false;
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (Vector2.Distance(toGridCoord(transform.position), TargetGridPos) ==0) {
            if (!reached) {
                reached = true;
                Debug.Log("Reach target");
                rigidbody.velocity = Vector2.zero;
                Explode();
                die();
            }
           
        }
        else {
            addForce(Vector2.down * Time.deltaTime * force);
        }
        //moveViaVelocity(Vector2.down, initialSpeed, accreleration);
        
    }

    private void Explode() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent<Tile>(out Tile tile)) {
                if (tile == this) {
                    continue;
                }

                if (tile.GetType() != typeof(Portal)) {
                    tile.takeDamage(this, 5, DamageType.Explosive);
                    tile.addForce((tile.transform.position - transform.position) * explodeForce);
                }
               
                
            }

        }
    }

    protected override void updateSpriteSorting() {
        _sprite.sortingOrder = 10;
    }
}
