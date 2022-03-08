using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimPortalGun : Tile
{
    public GameObject bulletPrefab;
    public float recoilForce = 100;
    public float shootForce = 1000f;
    private int remainingShoot = 2;

    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform shootPosition;


    public Portal EntrancePortal;
    public Portal ExitPortal;
	protected virtual void Update()
    {
      
        if (_tileHoldingUs != null) {
            aim();
        }
        else {
            _sprite.transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        updateSpriteSorting();
    }

    public override void useAsItem(Tile tileUsingUs) {
        if (remainingShoot > 0) {
            aim();
            tileUsingUs.addForce(-recoilForce * tileUsingUs.aimDirection.normalized);

            remainingShoot--;

            GameObject newBullet = Tile.spawnTile(bulletPrefab, tileUsingUs.transform.parent, (int)localX, (int)localY).gameObject;
            newBullet.transform.position = shootPosition.position;
            newBullet.transform.rotation = transform.rotation;
            newBullet.GetComponent<Tile>().addForce(tileUsingUs.aimDirection.normalized * shootForce);

            PortalBullet portalBullet = newBullet.GetComponent<PortalBullet>();

            if (portalBullet)
            {
                portalBullet.PortalGun = this;
                if (remainingShoot == 1)
                {
                    portalBullet.PortalType = PortalType.Entrance;
                }
                else
                {
                    portalBullet.PortalType = PortalType.Exit;
                }
            }
        }


        if (remainingShoot <= 0) {
            //die();
            _sprite.enabled = false;
            Destroy(this.gameObject, 2f);
        }
    }

    public void PortalSetup(Portal portal, PortalType portalType) {
        if (portalType == PortalType.Entrance) {
            EntrancePortal = portal;
        }
        else {
            ExitPortal = portal;
            //link portals
            if (EntrancePortal) {
                EntrancePortal.LinkedPortal = ExitPortal;
                ExitPortal.LinkedPortal = EntrancePortal;
            }
            
        }

        
    }

    protected void aim()
    {
        //AP's code
        _sprite.transform.localPosition = new Vector3(1f, 0, 0);
        float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
        if (_tileHoldingUs.aimDirection.x < 0) {
            _sprite.flipY = true;
        }
        else {
            _sprite.flipY = false;
        }
    }
}
