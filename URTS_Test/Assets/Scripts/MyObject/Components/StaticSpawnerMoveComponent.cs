using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSpawnerMoveComponent : MoveComponent
{
    SpawnComponent spawnComponent;
    SpawnComponentTutorial SpawnComponentTutorial;

    override public void SetAgentDestination(Vector3 destination) {
        if(spawnComponent == null) {
            spawnComponent = GetComponent<SpawnComponent>();

            if(spawnComponent == null) {
                SpawnComponentTutorial = GetComponent<SpawnComponentTutorial>();
                if (SpawnComponentTutorial == null) {
                    Debug.LogError("StaticSpawnerMoveComponent error: No spawnComponent found");
                    return;
                }
            }
        }

        if (spawnComponent != null)
            spawnComponent.waypointLocation.position = destination;
        else
            SpawnComponentTutorial.waypointLocation.position = destination;
    }

}
