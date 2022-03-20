using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenArmoredTurtle : WenEnemy
{
	// How close we need to be to our target before we're basically there
	public const float GRID_SNAP_THRESHOLD = 0.1f;

	// When we move, we try to move to grid snapped locations, so our current target
	// is stored in grid coordinates.
	protected Vector2 _targetGridPos;
	public Vector2 targetGridPos
	{
		get { return _targetGridPos; }
	}

	protected bool _takingCorrectingStep = false;
	public bool takingCorrectingStep
	{
		get { return _takingCorrectingStep; }
	}


	// We move similar to how the player moves, so we keep similar tunable values.
	public float moveSpeed = 5;
	public float moveAcceleration = 100;

	// This doesn't NEED to imply you'll ATTACK these tags. You'll maybe just follow them around.
	// But this can be useful in case you want to have an enemy switch sides and become a friend.
	public TileTags tagsWeChase = TileTags.Friendly;


	// A lot of the AIs will use a list of neighboring positions at some point. Here's a shared list for that so you don't have to allocate too many of them.
	protected static List<Vector2> _neighborPositions = new List<Vector2>(8);

	public bool isDefending;

	public Sprite defendSprite, shootSprite;

	public virtual void Start()
	{
		_targetGridPos = Tile.toGridCoord(globalX, globalY);
		InitializeEnemy();
	}

	public virtual void FixedUpdate()
	{
		Vector2 targetGlobalPos = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
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




		if(Detect(player.gameObject, 10))
        {
			isDefending = true;
			gameObject.GetComponent<SpriteRenderer>().sprite = defendSprite;
        }
		else if(Detect(player.gameObject, 20))
        {
			isDefending = false;
			gameObject.GetComponent<SpriteRenderer>().sprite = shootSprite;
			if (fireInterval <= 0)
			{
				Fire();
			}
			fireInterval -= Time.deltaTime;

			if (fireInterval < 0.2f)
			{
				_sprite.enabled = false;
			}
			else
			{
				_sprite.enabled = true;
			}
		}
        else
        {
			gameObject.GetComponent<SpriteRenderer>().sprite = shootSprite;
			isDefending = false;
        }

	}


	protected virtual void takeStep()
	{
		// Here's the function you can override to figure out how your AI object moves.
		// takeStep will USUALLY set _targetGridPos to do this. 
		_targetGridPos = Tile.toGridCoord(globalX, globalY);
	}

	public GameObject bulletPrefab;
	public float fireInterval;

	protected void Fire()
	{
		aimDirection = (player.gameObject.transform.position - transform.position).normalized;
		float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		GameObject newBullet = Instantiate(bulletPrefab);
		newBullet.transform.parent = transform.parent;
		newBullet.transform.position = transform.position;
		newBullet.transform.rotation = Quaternion.Euler(0, 0, aimAngle);

		Tile newBulletTile = newBullet.GetComponent<Tile>();


		newBulletTile.init();
		newBulletTile.sprite.sortingOrder = sprite.sortingOrder + 1;
		newBulletTile.sprite.sortingLayerID = SortingLayer.NameToID("Air");
		newBulletTile.addForce(aimDirection * Random.Range(500, 1000));


		Physics2D.IgnoreCollision(mainCollider, newBullet.GetComponent<Tile>().mainCollider);

		fireInterval = Random.Range(1f, 2f);
	}

    public override void takeDamage(Tile tileDamagingUs, int damageAmount, DamageType damageType)
    {
		if (isDefending)
		{

		}
		else
		{
			base.takeDamage(tileDamagingUs, damageAmount, damageType);
		}
    }
}
