using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCart : Tile
{
    bool isHurt;
    public Sprite spikeOn, spikeOff;

    public void Start()
    {
        isHurt = false;
        StartCoroutine(SpikeCartLoop());
    }

    public IEnumerator SpikeCartLoop()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        isHurt = false;

        Vector2 direction = Vector2.zero;
        GameObject player = GameObject.Find("player_tile(Clone)");

        if((transform.position - player.transform.position).magnitude <= 30)
        {
            direction = (player.transform.position - transform.position).normalized;
        }
        
        gameObject.GetComponent<Rigidbody2D>().AddForce(direction * 1000f);
        yield return SpikeCartLoop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHurt)
        {
            return;
        }
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            isHurt = true;
            collision.gameObject.GetComponent<Player>().takeDamage(collision.gameObject.GetComponent<Tile>(), 1);
        }
    }

}
