using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnComponent))]
public class AISpawnComponent : MonoBehaviour
{
    [HideInInspector] public float spawntimerMax = 8f;
    [HideInInspector] public float spawntimerCurr = 0f;

    [Tooltip("Any Scriptable Object must also exist in the SpawnComponent")]
    public List<InfantryUnit> spawnOrder;
    private int currentIndex = 0;

    SpawnComponent spawnComponent;

    private void Start() {
        spawnComponent = GetComponent<SpawnComponent>();
    }

    private void Update() {
        spawntimerCurr += Time.deltaTime;
        if(spawntimerCurr >= spawntimerMax) {
            spawntimerCurr = 0;
            spawnComponent.infantryUnitToSpawnQueueItem[spawnOrder[currentIndex]].EnqueuePrefab();
            currentIndex++;
            if(currentIndex >= spawnOrder.Count) {
                currentIndex = 0;
            }
        }
    }
}
