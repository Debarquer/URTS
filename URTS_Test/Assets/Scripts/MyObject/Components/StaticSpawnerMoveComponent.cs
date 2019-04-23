using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnComponent))]
public class StaticSpawnerMoveComponent : MoveComponent
{
    SpawnComponent spawnComponent;

    override public void SetAgentDestination(Vector3 destination) {
        if(spawnComponent == null) {
            spawnComponent = GetComponent<SpawnComponent>();

            if(spawnComponent == null) {
                Debug.LogError("StaticSpawnerMoveComponent error: No spawnComponent found");
                return;
            }
        }
        spawnComponent.waypointLocation.position = destination;
    }

}
