using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableComponent : MonoBehaviour
{
    public GameObject selectionIndicator;
    public GameObject rangeIndicator;
    public Renderer waypointRenderer;

    public delegate void SelectDelegate();
    public event SelectDelegate OnSelect;

    public delegate void DeselectDelegate();
    public event DeselectDelegate OnDeselect;

    private void Start() {
        GetComponent<MyObject>().OnActivate += FirstDeselect;
    }

    private void FirstDeselect() {
        GetComponent<MyObject>().OnActivate -= FirstDeselect;
        Deselect();
    }

    public void Select() {

        OnSelect?.Invoke();

        if(selectionIndicator != null)
            selectionIndicator.SetActive(true);
        if(rangeIndicator != null) {
            rangeIndicator.SetActive(true);
        }
        if (waypointRenderer != null) {
            waypointRenderer.enabled = true;
        }
    }

    public void Deselect() {

        OnDeselect?.Invoke();

        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
        if (rangeIndicator != null) {
            rangeIndicator.SetActive(false);
        }
        if (waypointRenderer != null) {
            waypointRenderer.enabled = false;
        }
    }
}
