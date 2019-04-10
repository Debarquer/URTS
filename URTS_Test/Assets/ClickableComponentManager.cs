using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponentManager : MonoBehaviour
{
    public static ClickableComponentManager instance;

    void Start() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    List<ClickableComponent> clickableComponents = new List<ClickableComponent>();

    public void AddClickableComponent(ClickableComponent clickableComponent) {
        clickableComponents.Add(clickableComponent);
    }

    public void RemoveClickableComponent(ClickableComponent clickableComponent) {
        clickableComponents.Remove(clickableComponent);
    }

    public ClickableComponent[] GetClickableComponents() {
        return clickableComponents.ToArray();
    }

    public void UnclickAllClickableComponents() {
        foreach(ClickableComponent clickableComponent in clickableComponents) {
            clickableComponent.Unclick();
        }
    }
}
