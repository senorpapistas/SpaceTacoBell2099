using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class leHitbox : MonoBehaviour
{
    [SerializeField] private float xKB, yKB, timeStop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            if (!other.transform.GetComponent<leEnemy>().invincible)
            {
                other.transform.GetComponent<Rigidbody>().AddForce(new Vector3((other.transform.position.x - transform.parent.position.x) * xKB, Mathf.Abs(other.transform.position.y - transform.position.y - .5f) * yKB, 0f), ForceMode.Impulse);
                StartCoroutine(other.transform.GetComponent<leEnemy>().getHit());
                Time.timeScale = 0f;
                StartCoroutine(timeFreeze());
            }
            
        }
    }

    private IEnumerator timeFreeze()
    {
        yield return new WaitForSecondsRealtime(timeStop);
        Time.timeScale = 1f;
    }
}
