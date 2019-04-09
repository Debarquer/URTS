using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpawnComponent : MonoBehaviour {
    float spawntimerMax = 5f;
    float spawntimerCurr = 0f;

    public MyObject UnitPrefab;
    Queue<MyObject> spawnQueue = new Queue<MyObject>();

    private void Update() {

        if(Input.GetKeyDown(KeyCode.O)){
            EnqueuePrefab();
        }

        if(spawnQueue.Count > 0) {
            spawntimerCurr += Time.deltaTime;

            Debug.Log(spawntimerCurr);

            if (spawntimerCurr > spawntimerMax) {

                spawntimerCurr = 0;

                MyObject tmp = Instantiate(spawnQueue.Dequeue(), transform.position + Random.insideUnitSphere, Quaternion.identity);
                tmp.team = GetComponent<MyObject>().team;
            }
        }
        else {
            spawntimerCurr = 0;
        } 
    }

    public void EnqueuePrefab() {
        spawnQueue.Enqueue(UnitPrefab);

        Debug.Log("Enqueue-ing");
    }
}