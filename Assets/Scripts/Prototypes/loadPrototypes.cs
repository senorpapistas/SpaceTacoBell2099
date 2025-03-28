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
    // movement prototypes
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
    // not movement prototypes
    public void LoadRalphInventoryProt()
    {
        SceneManager.LoadScene("RalphInventory", LoadSceneMode.Single);
    }
    public void LoadEwinCombatProt()
    {
        SceneManager.LoadScene("PotOfGreen", LoadSceneMode.Single);
    }
    public void LoadSkeleton2()
    {
        SceneManager.LoadScene("REPLACE_ME", LoadSceneMode.Single);
    }
}
