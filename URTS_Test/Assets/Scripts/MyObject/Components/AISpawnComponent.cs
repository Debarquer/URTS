using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnComponent))]
public class AISpawnComponent : MonoBehaviour
{
    public float spawntimerMax = 8f;
    [HideInInspector] public float spawntimerCurr = 0f;

    [Tooltip("Any Scriptable Object must also exist in the SpawnComponent")]
    public List<InfantryUnit> spawnOrder;
    private int currentIndex = 0;

    SpawnComponent spawnComponent;

    public List<AIMoveComponent> aIMoveComponents = new List<AIMoveComponent>();

    private void OnEnable() {
        spawnComponent = GetComponent<SpawnComponent>();
        spawnComponent.OnUnitSpawned += AddNewUnit;
    }

    private void OnDisable() {
        spawnComponent.OnUnitSpawned -= AddNewUnit;
    }

    void AddNewUnit(MyObject myObject) {
        aIMoveComponents.Add(myObject.GetComponent<AIMoveComponent>());
        if(aIMoveComponents.Count >= 6) {
            foreach(AIMoveComponent aIMoveComponent in aIMoveComponents) {
                aIMoveComponent.MoveToHQ();
            }

            aIMoveComponents.Clear();
        }
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
