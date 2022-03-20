using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WenSniper : WenEnemy
{
    bool isAiming;
    float aimingTime;
    public GameObject aimTarget;

    public GameObject attachedAimTarget;
    private void Start()
    {
        InitializeEnemy();
    }

    private void Update()
    {
        if (Detect(player.gameObject, 25f))
        {
            if (!isAiming)
            {
                StartAiming();
            }
            else if(isAiming)
            {
                aimingTime -= Time.deltaTime;
                attachedAimTarget.GetComponent<SpriteRenderer>().color = new Color(1, 1 - ((15 - aimingTime) / 15), 1 - ((15 - aimingTime) / 15));
                if (aimingTime <= 0)
                {
                    Shoot();
                }
            }
        }
        else
        {
            Destroy(attachedAimTarget);
            StopAiming();
        }
    }

    void StartAiming()
    {
        aimingTime = 15f;
        isAiming = true;
        attachedAimTarget = Instantiate(aimTarget);
    }

    void StopAiming()
    {
        isAiming = false;
    }

    void Shoot()
    {
        aimingTime = 15f;
        player.takeDamage(player, 1);
    }
}
