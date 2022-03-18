using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureWall : MonoBehaviour
{
    public List<GameObject> restSupplyList;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0);
    }

    private void OnDestroy()
    {
        int index = Random.Range(0, restSupplyList.Count);
        Instantiate(restSupplyList[index], transform.position, Quaternion.identity);
    }
}
