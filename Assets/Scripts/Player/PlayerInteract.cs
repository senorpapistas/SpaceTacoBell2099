using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public List<GameObject> interactableList;

    private KeyCode INTERACT_KEY = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(INTERACT_KEY))
        {
            interactableList[0].GetComponent<Interactable>().Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Interactable")
        {
            interactableList.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Interactable")
        {
            interactableList.Remove(collider.gameObject);
        }
    }
}
