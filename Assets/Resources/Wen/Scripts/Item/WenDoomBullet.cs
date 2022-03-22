using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenDoomBullet : Tile
{
	public float onGroundThreshold = 1f;

	public float _destroyTimer = 20f;

	protected ContactPoint2D[] _contacts = null;

	void Start()
	{
		_contacts = new ContactPoint2D[10];
		if (GetComponent<TrailRenderer>() != null)
		{
			GetComponent<TrailRenderer>().Clear();
		}
	}

	void Update()
	{
		// If we're moving kinda slow now we can just delete ourselves.
		if (_body.velocity.magnitude <= onGroundThreshold)
		{
			_destroyTimer -= Time.deltaTime;
			if (_destroyTimer <= 0)
			{
				die();
			}
		}
		transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.GetComponent<Tile>() != null)
		{
			if (collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Wall)
				|| collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Enemy)
				|| collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Creature))
			{
				collision.gameObject.GetComponent<Tile>().takeDamage(collision.gameObject.GetComponent<Tile>(), 99999, DamageType.Explosive);
			}
		}
	}
}

