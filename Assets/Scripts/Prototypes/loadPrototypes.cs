using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadPrototypes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadIvanPrototype()
    {
        SceneManager.LoadScene("Ivan", LoadSceneMode.Single);
    }

    public void LoadRalphPrototype()
    {
        SceneManager.LoadScene("RalphScene", LoadSceneMode.Single);
    }

    public void LoadEwinPrototype()
    {
        SceneManager.LoadScene("LeBrainJar", LoadSceneMode.Single);
    }
}
