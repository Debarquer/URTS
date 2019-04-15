using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableComponent : MonoBehaviour
{
    public GameObject selectionIndicator;
    public GameObject rangeIndicator;

    private void Start() {
        GetComponent<MyObject>().OnActivate += FirstDeselect;
    }

    private void FirstDeselect() {
        GetComponent<MyObject>().OnActivate -= FirstDeselect;
        Deselect();
    }

    public void Select() {
        selectionIndicator.SetActive(true);
        if(rangeIndicator != null) {
            rangeIndicator.SetActive(true);
        }
    }

    public void Deselect() {
        selectionIndicator.SetActive(false);
        if (rangeIndicator != null) {
            rangeIndicator.SetActive(false);
        }
    }
}
