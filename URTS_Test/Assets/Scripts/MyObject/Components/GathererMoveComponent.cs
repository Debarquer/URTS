using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class GathererMoveComponent : MoveComponent
{

    public float maxMinerals = 100;
    public float currentMinerals = 0;

    ResourceNode nearestResourceNode = null;
    GameObject nearestResourceNodeGameobject;
    private RefineryGatheringPoint nearestRefineryGetheringPoint;
    public float gatheringRange;
    public float gatheringRate;
    public float resourceDumpSpeedFactor = 4;

    public bool returningToBase = false;
    private GameManager gameManager;

    void Update()
    {
        if(!returningToBase) {
            if(nearestResourceNode == null || nearestResourceNodeGameobject == null) {
                if (FindNearestResourceNode()) {
                    MoveTowardsNearestResourceNode();
                }
                else {
                    // No resource node found
                    return;
                }
            }
            else {
                MoveTowardsNearestResourceNode();
            }
        }
        else {
            if (nearestRefineryGetheringPoint == null) {
                if (FindNearestRefinery()) {
                    MoveTowardsNearestRefinery();
                }
                else {
                    // No resource node found
                    return;
                }
            }
            else {
                MoveTowardsNearestRefinery();
            }
            
        }
    }

    private void MoveTowardsNearestResourceNode() {
        if(Vector3.Distance(transform.position, nearestResourceNodeGameobject.transform.position) <= gatheringRange) {

            float delta = gatheringRate * Time.deltaTime;
            currentMinerals += delta;
            nearestResourceNode.Mine(delta);

            if (currentMinerals >= maxMinerals) {
                currentMinerals = maxMinerals;
                nearestResourceNode.FreeUpGathererPoint(nearestResourceNodeGameobject);

                nearestResourceNode = null;
                nearestResourceNodeGameobject = null;

                returningToBase = true;
            }
        }
    }

    private void MoveTowardsNearestRefinery() {
        if (Vector3.Distance(transform.position, nearestRefineryGetheringPoint.transform.position) <= gatheringRange) {

            float delta = gatheringRate * resourceDumpSpeedFactor * Time.deltaTime;
            currentMinerals -= delta;
            if(gameManager == null) {
                gameManager = FindObjectOfType<GameManager>();
            }

            if(currentMinerals > 0) {
                gameManager.UpdateMinerals(delta);
            }
            if (currentMinerals <= 0) {
                nearestRefineryGetheringPoint.FreeUp();
                currentMinerals = 0;

                returningToBase = false;
            }
        }
    }

    private bool FindNearestResourceNode() {

        ResetNearestVars();

        ResourceNode[] resourceNodes = FindObjectsOfType<ResourceNode>();
        if(resourceNodes != null && resourceNodes.Length > 0) {
            //nearestResourceNode = resourceNodes.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();

            foreach(ResourceNode resourceNode in resourceNodes.OrderBy(x => Vector3.Distance(transform.position, x.transform.position))) {
                nearestResourceNodeGameobject = resourceNode.GetGathererPoint();
                if(nearestResourceNodeGameobject != null) {
                    nearestResourceNode = resourceNode;
                    GetComponent<NavMeshAgent>().destination = nearestResourceNodeGameobject.transform.position;
                    return true;
                }
            }

            returningToBase = true;
            return false;
        }
        else {
            returningToBase = true;
            return false;
        }
    }  

    private bool FindNearestRefinery() {
        ResetNearestVars();

        RefineryGatheringPoint[] refineryGatheringPoints = FindObjectsOfType<RefineryGatheringPoint>();
        if(refineryGatheringPoints != null && refineryGatheringPoints.Length > 0) {
            foreach (RefineryGatheringPoint refineryGatheringPoint in refineryGatheringPoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position))) {

                if (refineryGatheringPoint.IsAvailable()) {
                    refineryGatheringPoint.Occupy();
                    nearestRefineryGetheringPoint = refineryGatheringPoint;
                    GetComponent<NavMeshAgent>().destination = nearestRefineryGetheringPoint.transform.position;
                    return true;
                }
            }

            return false;
        }
        else {
            return false;
        }
    }

    private void ResetNearestVars() {
        nearestResourceNode = null;
        nearestResourceNodeGameobject = null;
        nearestRefineryGetheringPoint = null;
    }
}
