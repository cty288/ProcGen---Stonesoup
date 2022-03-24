using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenAimTarget : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = GameObject.Find("player_tile(Clone)").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        GetComponent<SpriteRenderer>().sortingOrder++;
    }
}
