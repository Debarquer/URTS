using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnUIButtonScript : EventTrigger
{
    public override void OnPointerClick(PointerEventData data) {
        if(data.button == PointerEventData.InputButton.Right) {
            Debug.Log("OnPointerClick called.");

            if(transform.GetComponentInParent<SpawnComponent>().currentSpawnQueueItem != null)
                transform.GetComponentInParent<SpawnComponent>().currentSpawnQueueItem.Cancel();
        }
    }
}
