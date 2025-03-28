using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator playerAnim;
    private bool swinging;
    private float horizontal;
    [SerializeField] private GameObject swingHitbox;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInParent<Animator>();
        swinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0 && !swinging) { transform.localScale = new Vector3(.9f, transform.localScale.y, transform.localScale.y); }
        else if (horizontal < 0 && !swinging) { transform.localScale = new Vector3(-.9f, transform.localScale.y, transform.localScale.y); }

        if (Input.GetMouseButtonDown(0) && !swinging)
        {
            swinging = true;
            swingHitbox.SetActive(true);
            playerAnim.SetTrigger("swinging");
            Debug.Log("should be swinging");
            StartCoroutine("swingCD");
        }
    }

    private IEnumerator swingCD()
    {
        yield return new WaitForSeconds(.4f);
        swingHitbox.SetActive(false);
        yield return new WaitForSeconds(.1f);
        swinging = false;
    }
}
