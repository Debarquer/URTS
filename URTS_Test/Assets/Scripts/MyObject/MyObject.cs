using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyObjectType { Light, Heavy, VHeavy };

public class MyObject : MonoBehaviour, IAttackable
{
    int owner;
    float moveSpeed;
    public int cost;
    // graphics
    public float currHealth;
    public float maxHealth = 5;
    string myName;
    string description;
    public MyObjectType myObjectType;
    public team team;
    public int powerReq;

    public delegate void GameObjectDisableDelegate(MyObject disabledObject);
    public event GameObjectDisableDelegate OnGameObjectDisabled;

    public delegate void ActivateDelegate();
    public event ActivateDelegate OnActivate;

    public delegate void DisableDelegate();
    public event DisableDelegate OnMyObjectDisable;

    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate() {

        Debug.Log("I am activate", this);
        active = true;
        OnActivate?.Invoke();
    }

    public void Disable() {
        active = false;
        OnMyObjectDisable?.Invoke();
    }

    private void OnEnable() {
        FindObjectOfType<GameManager>().UpdatePower(-powerReq);   
    }

    private void OnDisable() {
        OnGameObjectDisabled?.Invoke(this);
        FindObjectOfType<GameManager>().UpdatePower(powerReq);
    }

    void Heal() {
        throw new System.NotImplementedException();
    }

    virtual public void LoadFromScriptableObject<T>(T scriptableObject) where T : MySO {
        throw new System.NotImplementedException();
    }

    public void ReceiveDamage(int damage) {
        currHealth -= damage;

        float t = 1 - (float)currHealth / (float)maxHealth;

        Debug.Log(Color.Lerp(Color.green, Color.green, t));
        GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.green, Color.red, t);

        if(currHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
