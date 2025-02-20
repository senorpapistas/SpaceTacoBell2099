using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public Canvas debugMenu;
    

    [Header("Data")]
    public Rigidbody rb;
    public Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {debugMenu.enabled = !debugMenu.isActiveAndEnabled;}
    } 
}
