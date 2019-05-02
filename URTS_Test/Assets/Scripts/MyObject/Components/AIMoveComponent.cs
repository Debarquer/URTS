using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMoveComponent : MonoBehaviour
{

    NavMeshAgent navMeshAgent;

    public void MoveToHQ() {
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = GameObject.Find("HQ").transform.position;
    }
}
