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

    public delegate void DisableDelegate(MyObject disabledObject);
    public event DisableDelegate OnDisabled;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        FindObjectOfType<GameManager>().UpdatePower(-powerReq);   
    }

    private void OnDisable() {
        OnDisabled?.Invoke(this);
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
