using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A gate activated by a button
public class Gate : MonoBehaviour
{
    public Button button;

    // Update is called once per frame
    void Update()
    {
        if (button.activated) { ToggleGate(); }
    }

    public void ToggleGate()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
