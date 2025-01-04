using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] public bool open = true;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (open)
        {
            animator.Play("Gate_Open");
        }
        else
        {
            animator.Play("Gate_Close");
        }
    }

    // Start is called before the first frame update
    public void Open()
    {
        open = true;
        animator.Play("Gate_Open");
    }

    public void Close()
    {
        open = false;
        animator.Play("Gate_Close");
    }

    public void Toggle()
    {
        if (open)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void setState(bool state)
    {
        if (state)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
}
