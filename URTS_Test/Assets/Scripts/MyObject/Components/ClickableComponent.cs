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

        Debug.Log("I am a log");
        ClickableComponentManager.instance.AddClickableComponent(this);
    }

    private void OnDisable() {
        ClickableComponentManager.instance.RemoveClickableComponent(this);
    }

    public void Click() {
        Debug.Log("You clicked on " + name);

        OnClick?.Invoke();
    }

    public void Unclick() {
        OnUnClick?.Invoke();
    }
}
