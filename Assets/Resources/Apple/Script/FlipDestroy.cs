using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            Debug.Log("Destroy");
            Destroy(gameObject);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
