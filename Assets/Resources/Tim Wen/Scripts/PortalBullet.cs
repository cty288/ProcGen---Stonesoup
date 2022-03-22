using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBullet : Tile {
    [SerializeField] private GameObject portalPrefab;
    private bool hit = false;

    public TimPortalGun PortalGun;
    public PortalType PortalType;

    private void Start() {
        GetComponent<TrailRenderer>().Clear();
    }

    public virtual void OnCollisionEnter2D(Collision2D collision) {
       
        if (collision.gameObject.TryGetComponent<Tile>(out Tile tile) && !hit) {
            Vector2 position = new Vector2(tile.globalX, tile.globalY);
            if (tile.hasTag(TileTags.Wall)) {
                Destroy(tile.gameObject, 0.1f); //.takeDamage(this, 100);
            }

            hit = true;
            tile.takeDamage(tile, 1);
            //spawn portal

            Tile portal = this.SpawnTile(portalPrefab, tile.transform.parent, (int) position.x, (int) position.y);
            portal.gameObject.transform.position = new Vector3(position.x, position.y, 0);
            portal.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -9;
            portal.GetComponent<Portal>().PortalType = PortalType;

            if (PortalGun) {
                Debug.Log("Portal setup");
                PortalGun.PortalSetup(portal.GetComponent<Portal>(), PortalType);
            }
            die();
        }
    }
}
