using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WenSniper : WenEnemy
{
    bool isAiming;
    float aimingTime;

    private void Start()
    {
        InitializeEnemy();
    }

    private void Update()
    {
        if (Detect(player.gameObject, 50f))
        {
            if (!isAiming)
            {
                StartAiming();
            }
            else if(isAiming)
            {
                aimingTime -= Time.deltaTime;
                if (aimingTime <= 0)
                {
                    Shoot();
                }
            }
        }
        else
        {
            StopAiming();
        }
    }

    void StartAiming()
    {
        aimingTime = 5f;
        isAiming = true;
    }

    void StopAiming()
    {
        isAiming = false;
    }

    void Shoot()
    {
        aimingTime = 5f;
        player.takeDamage(player, 1);
    }
}
