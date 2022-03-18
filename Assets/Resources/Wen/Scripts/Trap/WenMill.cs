using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenMill : MonoBehaviour
{
    float rotateSpeed;

    void Update()
    {
        transform.localEulerAngles += new Vector3(0, 0, rotateSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            collision.GetComponent<Player>().takeDamage(collision.GetComponent<Tile>(), 1);
        }
    }
}
