using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leEnemy : MonoBehaviour
{
    public bool invincible;

    // Start is called before the first frame update
    void Start()
    {
        invincible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        invincible = false;
    }

    public IEnumerator getHit()
    {
        if (!invincible)
        {
            invincible = true;
            Debug.Log("enemy hit");
            yield return new WaitForSeconds(.3f);
            invincible = false;
            Debug.Log("enemy no longer invincible");
        }
    }
}
