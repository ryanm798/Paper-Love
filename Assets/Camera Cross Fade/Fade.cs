using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void FadeIn()
    {
        animator.enabled = true;
        animator.Play("CrossFade");
    }

    public void DisablePrevious()
    {
        animator.enabled = false;
        CrossFadeManager.Instance.DisablePrevious();
    }
}
