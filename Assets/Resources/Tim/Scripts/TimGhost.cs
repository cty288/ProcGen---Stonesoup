using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimGhost : BasicAICreature
{
    [SerializeField]
	protected Tile _tileWereChasing = null;
    public Tile tileWereChasing
    {
        get { return _tileWereChasing; }
        set { _tileWereChasing = value; }
    }

    public float maxDistanceToContinueChase = 8f;
    public float minteleportDistance = 5f;

    protected float _timeSinceLastStep = 0f;

    [SerializeField]
    private bool isTeleporting = false;
    private bool canTeleport = true;
    private bool canAttack = true;
    [SerializeField] private int damage = 1;

    private void Update() {
        if (!isTeleporting) {
            _timeSinceLastStep += Time.deltaTime;
            Vector2 targetGlobalPos = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
            float distanceToTarget = Vector2.Distance(transform.position, targetGlobalPos);
            if (_timeSinceLastStep >= 0.5f)
            {
                takeStep();
            }
		}
       
    }

	protected override void takeStep()
	{
		//AP's Code
        _timeSinceLastStep = 0f;

		if (_tileWereChasing == null)
		{
			_targetGridPos = toGridCoord(globalX +Random.Range(-5f,5f), globalY + Random.Range(-5f,5f));
			return;
		}

		// First, figure out if the target is too far away
		float distanceToTile = Vector2.Distance(transform.position, _tileWereChasing.transform.position);
		if (distanceToTile > maxDistanceToContinueChase)
		{
			_tileWereChasing = null;
			return;
		}

		// We do this to re-calculate exactly where we are right now. 
		_targetGridPos = Tile.toGridCoord(globalX, globalY);

		_neighborPositions.Clear();

		// Otherwise, we're going to look at all potential neighbors and then figure out the best one to go to.
		Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y + 1);
		if (pathIsClear(toWorldCoord(upGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(upGridNeighbor);
		}
		Vector2 upRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y + 1);
		if (pathIsClear(toWorldCoord(upRightGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(upRightGridNeighbor);
		}
		Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(rightGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(rightGridNeighbor);
		}
		Vector2 downRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y - 1);
		if (pathIsClear(toWorldCoord(downRightGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(downRightGridNeighbor);
		}
		Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y - 1);
		if (pathIsClear(toWorldCoord(downGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(downGridNeighbor);
		}
		Vector2 downLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y - 1);
		if (pathIsClear(toWorldCoord(downLeftGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(downLeftGridNeighbor);
		}
		Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(leftGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(leftGridNeighbor);
		}
		Vector2 upLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y + 1);
		if (pathIsClear(toWorldCoord(upLeftGridNeighbor), CanOverlapIgnoreTargetTile))
		{
			_neighborPositions.Add(upLeftGridNeighbor);
		}

		// Now, of the neighbor positions, pick the one that's closest. 
		float minDistance = distanceToTile;
		Vector2 minNeighbor = _targetGridPos;
		GlobalFuncs.shuffle(_neighborPositions);
		foreach (Vector2 neighborPos in _neighborPositions)
		{
			float distanceFromTarget = Vector2.Distance(Tile.toWorldCoord(neighborPos.x, neighborPos.y), _tileWereChasing.transform.position);
			if (distanceFromTarget < minDistance)
			{
				minNeighbor = neighborPos;
				minDistance = distanceFromTarget;
			}
		}


		//End of AP's code
		if (minNeighbor == _targetGridPos)
		{
			// _tileWereChasing = null;
			//teleport
            StartTeleport();
            Debug.Log("Teleport");
		}

        if (minDistance >= minteleportDistance) {
			//teleport
			StartTeleport();
			Debug.Log("Teleport");
        }


		_targetGridPos = minNeighbor;

	}

    private Vector2 teleportPosition;
    private void StartTeleport() {
        if (tileWereChasing && canTeleport) {
            
            List<Vector2> availableSpaces = GetEmptySpaceNearPosition(2,
                new Vector2(Mathf.RoundToInt(tileWereChasing.transform.position.x), 
                    Mathf.RoundToInt(tileWereChasing.transform.position.y)));
            if (availableSpaces.Count > 0) {
                teleportPosition = GlobalFuncs.randElem(availableSpaces);
                canTeleport = false;
                isTeleporting = true;
                _anim.SetTrigger("Teleport");
                _collider.isTrigger = true;
			}
           
		}
       
    }

    private List<Vector2> GetEmptySpaceNearPosition(int radius, Vector2 center) {
        List<Vector2> availableSpaces = new List<Vector2>();
        Vector2 prevPos = transform.position;
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }

                Vector2 pos =center + new Vector2Int(x, y);
                transform.position = new Vector3(pos.x, pos.y);
                if (canSeeTile(tileWereChasing)) {
                    availableSpaces.Add(pos);
                }
            }
        }

        transform.position = prevPos;
		Debug.Log(availableSpaces.Count);
        return availableSpaces;
    }

    //AP's Code; modified
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTeleporting) {
            Tile otherTile = collision.gameObject.GetComponent<Tile>();

            // If we're chasing something, then take a step probably
            if (otherTile != _tileWereChasing
                && otherTile != null
                && otherTile.hasTag(TileTags.Creature))
            {

                BasicAICreature maybeOtherCreature = (otherTile as BasicAICreature);
                if (maybeOtherCreature != null
                    && maybeOtherCreature.targetGridPos == _targetGridPos
                    && (!_takingCorrectingStep || maybeOtherCreature.takingCorrectingStep)) {

                    StartTeleport();
                }

            }

            if (otherTile != null && otherTile.hasTag(tagsWeChase) && canAttack) {
                canAttack = false;
                otherTile.takeDamage(this, damage);
                Vector2 toOtherTile = (Vector2)otherTile.transform.position - (Vector2)transform.position;
                toOtherTile.Normalize();
                otherTile.addForce(1000 * toOtherTile);
                Invoke("ResetCanAttack", 2f);
            }
        }

    }

    void ResetCanAttack() {
        canAttack = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
	public void OnGhostTeleportMove() {
        List<Vector2> availableSpaces = GetEmptySpaceNearPosition(2,
            new Vector2(Mathf.RoundToInt(tileWereChasing.transform.position.x),
                Mathf.RoundToInt(tileWereChasing.transform.position.y)));
        if (availableSpaces.Count > 0)
        {
            teleportPosition = GlobalFuncs.randElem(availableSpaces);
        }

        transform.parent = tileWereChasing.transform.parent;
		transform.position = teleportPosition;
        _collider.isTrigger = false;
       Invoke("EndTeleport", 0.4f);
    }

    private void EndTeleport() {
        isTeleporting = false;
        _collider.isTrigger = false;
    }

    public void OnEndTeleport() {
        isTeleporting = false;
        _collider.isTrigger = false;
        tileWereChasing = null;
        StartCoroutine(TeleportWait());
    }

    IEnumerator TeleportWait() {
        yield return new WaitForSeconds(2f);
        canTeleport = true;
        isTeleporting = false;
    }
    public override void tileDetected(Tile otherTile)
    {
        if (_tileWereChasing == null && otherTile.hasTag(tagsWeChase))
        {
            _tileWereChasing = otherTile;
            takeStep();
        }
    }
	protected bool CanOverlapIgnoreTargetTile(RaycastHit2D hitResult)
    {
        Tile maybeResultTile = hitResult.transform.GetComponent<Tile>();
        if (maybeResultTile == _tileWereChasing)
        {
            return true;
        }
        return DefaultCanOverlapFunc(hitResult);
    }
}
