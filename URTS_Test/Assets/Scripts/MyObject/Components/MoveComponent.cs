using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class MoveComponent : MonoBehaviour {

    NavMeshAgent navMeshAgent;

    virtual public void SetAgentDestination(Vector3 destination) {
        if(navMeshAgent == null) {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if(navMeshAgent == null) {
                Debug.LogError("MoveComponent error: No navMeshAgent found");
                return;
            }
        }

        navMeshAgent.destination = destination;
    }
}