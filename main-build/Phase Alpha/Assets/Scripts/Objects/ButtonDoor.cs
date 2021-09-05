using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] private Animator door = null;
    [SerializeField] private Animator door2 = null;
    public void Button()
    {
        Debug.Log("pressed button");
        if (door.GetBool("close"))
        {
            Debug.Log("door was closed");
            //door.Play("open gate", 0, 0.0f);
            door.SetBool("close", false);
            door.SetBool("open", true);
        }
        if (door2 != null && door2.GetBool("close"))
        {
            Debug.Log("door was closed");
            //door.Play("open gate", 0, 0.0f);
            door2.SetBool("close", false);
            door2.SetBool("open", true);
        }
    }
}
