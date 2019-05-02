using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class MoveComponent : MonoBehaviour {

    NavMeshAgent navMeshAgent;
    public bool isMoving;

    public Animator animator;

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

    private void Update() {
        //if(animator != null) {
        //    if (navMeshAgent.remainingDistance > 2f) {
        //        animator.SetBool("isMoving", true);
        //    }
        //    else {
        //        animator.SetBool("isMoving", false);
        //    }
        //}
    }
}