using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnUIButtonTutorial : EventTrigger {
    bool onClickActive = false;
    public delegate void onClickDelegate();
    public onClickDelegate onClickDelegateFunc;

    public override void OnPointerClick(PointerEventData data) {
        if(data.button == PointerEventData.InputButton.Left) {

            Debug.Log("I am the left click", this);

            if (onClickActive) {
                Debug.Log("I am teh invokes");
                onClickDelegateFunc?.Invoke();
            }
        }
        if (data.button == PointerEventData.InputButton.Right) {
            Debug.Log("it is teh right click");

            if (transform.GetComponentInParent<SpawnComponent>().currentSpawnQueueItem != null)
                transform.GetComponentInParent<SpawnComponent>().currentSpawnQueueItem.Cancel();
        }
    }
}
