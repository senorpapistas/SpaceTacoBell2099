using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When player enters this trigger, switch to another vcam

public class CameraTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            VirtualCamera.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            VirtualCamera.gameObject.SetActive(false);
        }
    }
}
