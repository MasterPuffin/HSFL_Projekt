using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AgentController : MonoBehaviour
{
    public NavMeshAgent agent;
    private float distanceP1;
    private float distanceP2;
    private float distanceRandomPoint;
    private GameObject[] players;
    //private GameObject player2;
    public GameObject[] randomPoints;
    public GameObject currentPoint;
    private bool cr_running;
    private GameObject closestPlayer;

    private int index;

    private bool hasAggro;

    // Start is called before the first frame update
    void Start()
    {

        if (randomPoints.Length <= 0)
        {
            if (this.gameObject.name == "Monster")
            {
                randomPoints = GameObject.FindGameObjectsWithTag("MonsterWalkPoint");
            }
            else
            {
                randomPoints = GameObject.FindGameObjectsWithTag("MonsterWalkPointCanyons");
            }
            index = Random.Range(0, randomPoints.Length);
            currentPoint = randomPoints[index];
            agent.SetDestination(currentPoint.transform.position);
        }
        //player2 = GameObject.Find("Player(Clone)(1)");
    }

    // Update is called once per frame
    void Update()
    {
        distanceRandomPoint = Vector3.Distance(currentPoint.transform.position, transform.position);
        //Debug.Log(distanceRandomPoint);
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        else if (players.Length != 2)
        {
            players = GameObject.FindGameObjectsWithTag("Player");

        }
        else
        {
            distanceP1 = Vector3.Distance(players[0].transform.position, transform.position);
            distanceP2 = Vector3.Distance(players[1].transform.position, transform.position);
            
            
            if (distanceP1 < distanceP2)
            {
                closestPlayer = players[0];
            }
            else
            {
                closestPlayer = players[1];
            }

            if (Vector3.Distance(closestPlayer.transform.position, transform.position) < 5)
            {
                closestPlayer.GetComponent<PlayerController>().KillPlayer();
            }
            else if (Vector3.Distance(closestPlayer.transform.position, transform.position) < 15)
            {
                hasAggro = true;
            }
            else
            {
                hasAggro = false;
            }
        }
        if (hasAggro)
        {
            //agent.isStopped = false;
            agent.SetDestination(closestPlayer.transform.position);
        }
        else if (distanceRandomPoint <= 5.0f)
        {
            //Debug.Log("close to point");
            index = Random.Range(0, randomPoints.Length);
            currentPoint = randomPoints[index];
            
        }
        else
        {
            //Debug.Log("set new target point");
            agent.SetDestination(currentPoint.transform.position);
        }

    }

}
