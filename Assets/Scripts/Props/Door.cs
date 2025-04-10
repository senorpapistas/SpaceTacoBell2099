using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    //warps player to another door
    public GameObject otherDoor;

    /*
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.transform.position = otherDoor.transform.position;
        }
    }
    */

    public override void Interact()
    {
        GameObject.FindWithTag("Player").transform.position = otherDoor.transform.position;
    }
}
