using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    public delegate void OnClickDelegate();
    public event OnClickDelegate OnClick;

    public delegate void OnUnClickDelegate();
    public event OnUnClickDelegate OnUnClick;

    private void OnEnable() {
        if(ClickableComponentManager.instance != null)
            ClickableComponentManager.instance.AddClickableComponent(this);
    }

    private void OnDisable() {
        if (ClickableComponentManager.instance != null)
            ClickableComponentManager.instance.RemoveClickableComponent(this);
    }

    public void Click() {
        Debug.Log("I HAVE BEEN CLICKED", this);

        OnClick?.Invoke();
    }

    public void Unclick() {
        OnUnClick?.Invoke();
    }
}
