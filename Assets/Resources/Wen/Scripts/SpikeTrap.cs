using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Tile
{
    bool isOn, isHurt;
    public Sprite spikeOn, spikeOff;

    public void Start()
    {
        isOn = false;
        isHurt = false;
        StartCoroutine(SpikeLoop());
    }

    public IEnumerator SpikeLoop()
    {
        yield return new WaitForSeconds(3f);
        isOn = !isOn;
        if (isOn)
        {
            sprite.sprite = spikeOn;
            GetComponent<BoxCollider2D>().enabled = true;
            isHurt = false;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
            sprite.sprite = spikeOff;
        }
        yield return SpikeLoop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHurt)
        {
            return;
        }
        if (isOn && collision.GetComponent<Player>()!=null)
        {
            isHurt = true;
            collision.GetComponent<Player>().takeDamage(collision.GetComponent<Tile>(), 1);
        }
    }
}