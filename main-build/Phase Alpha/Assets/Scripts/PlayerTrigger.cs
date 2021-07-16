using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private bool triggered;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
        player = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            SetPlayer(other.gameObject);
            triggered = true;
        }

    }

    private void SetPlayer(GameObject player)
    {
        this.player = player;
        Debug.Log(player);
    }
    public GameObject GetPlayer()
    {
        return player;
    }
}