using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum PortalType {
    Entrance,
    Exit
}
public class Portal : Tile {
    public Portal LinkedPortal;
    public PortalType PortalType = PortalType.Entrance;

    public bool PlayerJustReach = false;

    [SerializeField] private Sprite exitPortalSprite;
    private void Start() {
        if (PortalType == PortalType.Exit) {
            sprite.sprite = exitPortalSprite;
            sprite.sortingOrder = -10;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (LinkedPortal) {
            if (!PlayerJustReach && (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                                     (other.gameObject.GetComponent<Tile>() && other.gameObject.GetComponent<Tile>().hasTag(TileTags.Creature))))
            {
                LinkedPortal.PlayerJustReach = true;
                other.gameObject.transform.position = LinkedPortal.gameObject.transform.position;
                other.gameObject.transform.parent = LinkedPortal.gameObject.transform.parent;
            }
        }
       
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            (other.gameObject.GetComponent<Tile>() && other.gameObject.GetComponent<Tile>().hasTag(TileTags.Creature))) {
            PlayerJustReach = false;
        }
    }

    private void Awake() {
        //GameManager.instance.currentRoom
    }
}
