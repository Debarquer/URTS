using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnUIButtonScript : EventTrigger
{
    public SpawnQueueItem SpawnQueueItem;
    public SpawnQueueItemTutorial SpawnQueueItemTutorial;

    public override void OnPointerClick(PointerEventData data) {
        if(data.button == PointerEventData.InputButton.Right) {
            Debug.Log("OnPointerClick called.");

            if (SpawnQueueItem != null)
                SpawnQueueItem.Cancel();
            else if (SpawnQueueItemTutorial != null)
                SpawnQueueItemTutorial.Cancel();
        }
    }
}
