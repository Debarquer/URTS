using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

    Rigidbody rb;
    public float forceBoost = 2f;
    public GoalScript gs;
    NavMeshAgent NavMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gs == null) {
            gs = FindObjectOfType<GoalScript>();
        }
        else {
            NavMeshAgent.destination = gs.transform.position;
        }
        //float vertical = Input.GetAxisRaw("Vertical");
        //float horizontal = Input.GetAxisRaw("Horizontal");

        //Vector3 force = new Vector3(horizontal, 0, vertical) * forceBoost;
        //rb.velocity = force;
    }
}
