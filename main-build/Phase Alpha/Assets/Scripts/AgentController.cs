using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public NavMeshAgent agent;
    private float distanceP1;
    private float distanceRandomPoint;
    private GameObject player1;
    private GameObject player2;
    public GameObject[] randomPoints;
    public GameObject currentPoint;

    private int index;

    private bool hasAggro;
    // Start is called before the first frame update
    void Start()
    {
        if (randomPoints.Length<=0)
        {
            
            randomPoints = GameObject.FindGameObjectsWithTag("MonsterWalkPoint");
            index = Random.Range(0, randomPoints.Length);
            currentPoint = randomPoints[index];
            agent.SetDestination(currentPoint.transform.position);
        }
        //player2 = GameObject.Find("Player(Clone)(1)");
    }

    // Update is called once per frame
    void Update()
    {
        if (player1 ==  null)
        {
            player1 = GameObject.Find("Player(Clone)");
        }
        else
        {
            distanceP1 = Vector3.Distance(player1.transform.position, transform.position);
            distanceRandomPoint = Vector3.Distance(currentPoint.transform.position, transform.position);

            if (distanceP1 < 5)
            {
                hasAggro = true;
            }
            else
            {
                hasAggro = false;
            }

            if (hasAggro)
            {
                //agent.isStopped = false;
                agent.SetDestination(player1.transform.position);
            }
            else if(distanceRandomPoint <= 3)
            {
                index = Random.Range(0, randomPoints.Length);
                //Debug.Log(index);
                currentPoint = randomPoints[index];
            }else
            {
                agent.SetDestination(currentPoint.transform.position);
            }
        }
        //Debug.Log(player1);
    }
}
