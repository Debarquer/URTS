using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public List<FlockAgent> agents;
    public FlockBehavior behavior;

    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(1f, 100f)] public float maximumSpeed = 10f;
    [Range(1f, 10f)] public float neighborRadius = 1.5f;
    [Range(0f, 1f)] public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maximumSpeed * maximumSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(FlockAgent agent in agents) {
            List<Transform> context = GetNearbyObjects(agent);
            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if(move.sqrMagnitude > squareMaxSpeed) {
                move = move.normalized * maximumSpeed;
            }

            agent.Move(move);
        }
    }

    private List<Transform> GetNearbyObjects(FlockAgent agent) {
        List<Transform> context = new List<Transform>();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach(Collider c in contextColliders) {
            if(c != agent.AgentCollider) {
                context.Add(c.transform);
            }
        }

        return context;
    }
}
