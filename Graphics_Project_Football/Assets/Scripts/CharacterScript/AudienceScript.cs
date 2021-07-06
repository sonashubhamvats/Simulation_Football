using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceScript : MonoBehaviour
{
    private Animator animator;

    private bool jump;
    private void Awake() {
        animator=GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enableJump());
    }

    // Update is called once per frame
    void Update()
    {
        if(jump)
        {
            animator.SetBool("jump",true);
        }
        else
        {
            animator.SetBool("jump",false);
        }
    }
    IEnumerator enableJump()
    {
        yield return new WaitForSeconds(Random.Range(0.5f,2f));
        jump=true;
        StartCoroutine(disableJump());
    }
    IEnumerator disableJump()
    {
        yield return new WaitForSeconds(Random.Range(0.5f,2f));
        jump=false;
        StartCoroutine(enableJump());

    }
}
