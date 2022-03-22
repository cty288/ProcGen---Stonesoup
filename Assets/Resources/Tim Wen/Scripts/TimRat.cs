using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimRat : BasicAICreature {
    private List<Vector2Int> alreadyMovedPosition = new List<Vector2Int>();
    
    [SerializeField] public float moveTimeInterval = 1f;
    private float moveTimer = 0f;

    private Vector2Int targetWorldPos;

    [SerializeField] private int maxDigNum = 10;

    private void Awake() {
        moveTimeInterval += Random.Range(0f, 1f);
    }

    public override void FixedUpdate() {
        Vector2 targetGlobalPos = targetWorldPos;
        if (Vector2.Distance(transform.position, targetGlobalPos) >= 0.1f)
        {
            // If we're away from our target position, move towards it.
            Vector2 toTargetPos = (targetGlobalPos - (Vector2)transform.position).normalized;
            moveViaVelocity(toTargetPos, moveSpeed, moveAcceleration);
            // Figure out which direction we're going to face. 
            // Prioritize side and down.
            if (_anim != null)
            {
                if (toTargetPos.x >= 0)
                {
                    _sprite.flipX = false;
                }
                else
                {
                    _sprite.flipX = true;
                }
                // Make sure we're marked as walking.
                _anim.SetBool("Walking", true);
                if (Mathf.Abs(toTargetPos.x) > 0 && Mathf.Abs(toTargetPos.x) > Mathf.Abs(toTargetPos.y))
                {
                    _anim.SetInteger("Direction", 1);
                }
                else if (toTargetPos.y > 0 && toTargetPos.y > Mathf.Abs(toTargetPos.x))
                {
                    _anim.SetInteger("Direction", 0);
                }
                else if (toTargetPos.y < 0 && Mathf.Abs(toTargetPos.y) > Mathf.Abs(toTargetPos.x))
                {
                    _anim.SetInteger("Direction", 2);
                }
            }
        }
        else
        {
            moveViaVelocity(Vector2.zero, 0, moveAcceleration);
            if (_anim != null)
            {
                _anim.SetBool("Walking", false);
            }
        }
    }

    public override void Start() {
        base.Start();
        targetWorldPos = new Vector2Int((int) transform.position.x, (int) transform.position.y);
        alreadyMovedPosition.Add(new Vector2Int((int) globalX, (int) globalY));
    }

    private void Update() {
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveTimeInterval) {
            moveTimer = 0;
            Vector2Int nextPos = GetNextTargetPosition();
            alreadyMovedPosition.Add(nextPos);
            targetWorldPos = nextPos;

            takeStep();
        }
    }

    private Vector2Int GetNextTargetPosition() {
        Vector2Int upGridNeighbor = new Vector2Int(Mathf.RoundToInt(globalX), Mathf.RoundToInt(globalY + TILE_SIZE));
        Vector2Int rightGridNeighbor = new Vector2Int(Mathf.RoundToInt(globalX + TILE_SIZE), Mathf.RoundToInt(globalY));
        Vector2Int downGridNeighbor = new Vector2Int(Mathf.RoundToInt(globalX),  Mathf.RoundToInt(globalY -  TILE_SIZE));
        Vector2Int leftGridNeighbor = new Vector2Int(Mathf.RoundToInt( globalX - TILE_SIZE), Mathf.RoundToInt(globalY));

        List<Vector2Int> possibleMovePos = new List<Vector2Int>()
            {upGridNeighbor, rightGridNeighbor, downGridNeighbor, leftGridNeighbor};

        possibleMovePos.RemoveAll(pos => pos.x < 1 || pos.x > 79 || pos.y < 1 || pos.y > 63);

        List<Vector2Int> unmovedPossiblePos = new List<Vector2Int>(possibleMovePos);

        foreach (Vector2Int alreadyMovedPos in alreadyMovedPosition) {
            unmovedPossiblePos.RemoveAll(pos => pos == alreadyMovedPos);
        }

        if (unmovedPossiblePos.Count > 0) {
            return GlobalFuncs.randElem(unmovedPossiblePos);
        }
        else {
            return GlobalFuncs.randElem(possibleMovePos);
        }
    
    }

    protected override void takeStep() {
        Vector2 toTarget = targetWorldPos - (Vector2)transform.position;
        float distanceToTarget = toTarget.magnitude;
        toTarget.Normalize();

        // Now we perform a collider "Cast" i.e. we simulate moving our collider forward along the path from us to our target. 
        // Any objects that the physics engine detects as collider with this simulated collider will be stored in _maybeRaycastResults

        // This is a kinda weird way of handling this. The Cast function returns an int instead of just returning the array of results (i.e. the thing we actually care about).
        // Why is it like this? Short answer is for performance. 
        int numCollisions = _collider.Cast(toTarget, _maybeRaycastResults, distanceToTarget);
        for (int i = 0; i < numCollisions && i < _maybeRaycastResults.Length; i++)
        {
            if (_maybeRaycastResults[i].collider.TryGetComponent<Tile>(out Tile tile))
            {

                if (tile.hasTag(TileTags.Wall)) {
                    maxDigNum--;
                    Destroy(tile.gameObject, 0.1f);
                }

                if (maxDigNum <= 0) {
                    die();
                }
            }
        }
    }
}
