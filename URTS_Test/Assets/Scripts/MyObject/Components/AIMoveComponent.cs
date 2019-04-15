using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMoveComponent : MonoBehaviour
{

    NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = GameObject.Find("HQ").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
