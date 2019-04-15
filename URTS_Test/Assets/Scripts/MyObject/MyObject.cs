﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject : MonoBehaviour
{
    int owner;
    float moveSpeed;
    public int cost;
    // graphics
    //public float currHealth;
    //public float maxHealth = 5;
    string myName;
    string description;
    public MyObjectType myObjectType;
    public Team team;
    public int powerReq;

    public delegate void GameObjectDisableDelegate(MyObject disabledObject);
    public event GameObjectDisableDelegate OnGameObjectDisabled;

    public delegate void ActivateDelegate();
    public event ActivateDelegate OnActivate;

    public delegate void DisableDelegate();
    public event DisableDelegate OnMyObjectDisable;

    public bool active = false;
    public bool placed = false;

    public void OnPlaced() {
        if(team == Team.A) {
            FindObjectOfType<GameManager>().UpdatePower(-powerReq);
            placed = true;
        }
    }

    public void Activate() {
        if(powerReq > 0 && FindObjectOfType<GameManager>().GetPower() <= 0) {
            // Not enough power
        }
        else {
            if(name == "Radar(Clone)")
                Debug.Log("Activating: " + name);
            active = true;
            OnActivate?.Invoke();
        }     
    }

    public void Disable() {
        active = false;
        OnMyObjectDisable?.Invoke();
    }

    private void OnEnable() {
    }

    private void OnDisable() {
        OnGameObjectDisabled?.Invoke(this);
        GameManager gameManager = FindObjectOfType<GameManager>();
        if(gameManager != null && placed && team == Team.A) {
            gameManager.UpdatePower(powerReq);
        }
    }

    void Heal() {
        throw new System.NotImplementedException();
    }

    virtual public void LoadFromScriptableObject<T>(T scriptableObject) where T : MySO {
        throw new System.NotImplementedException();
    }
}
