﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    public delegate void OnClickDelegate();
    public event OnClickDelegate OnClick;

    public delegate void OnUnClickDelegate();
    public event OnUnClickDelegate OnUnClick;

    private void OnEnable() {
        ClickableComponentManager.instance.AddClickableComponent(this);
    }

    private void OnDisable() {
        ClickableComponentManager.instance.RemoveClickableComponent(this);
    }

    public void Click() {
        OnClick?.Invoke();
    }

    public void Unclick() {
        OnUnClick?.Invoke();
    }
}
