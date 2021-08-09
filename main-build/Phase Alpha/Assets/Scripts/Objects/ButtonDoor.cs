using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] private Animator door = null;
    // Start is called before the first frame update
    void Start()
    {
        if (door.GetBool("close"))
        {
            door.Play("open gate", 0, 0.0f);
            door.SetBool("close", false);
            door.SetBool("open", true);
        }
    }
}
