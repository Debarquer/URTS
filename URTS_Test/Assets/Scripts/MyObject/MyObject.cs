using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyObjectType { Light, Heavy, VHeavy };

public class MyObject : MonoBehaviour, IAttackable
{
    int owner;
    float moveSpeed;
    int cost;
    // graphics
    int currHealth;
    int maxHealth;
    string myName;
    string description;
    public MyObjectType myObjectType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Heal() {
        throw new System.NotImplementedException();
    }

    virtual public void LoadFromScriptableObject<T>(T scriptableObject) where T : MySO {
        throw new System.NotImplementedException();
    }

    public void ReceiveDamage(int damage) {
        throw new System.NotImplementedException();
    }
}
