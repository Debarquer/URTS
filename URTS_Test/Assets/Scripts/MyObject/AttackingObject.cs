using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingObject : MyObject
{
    int range;
    float attackCooldownMax;
    float attackCooldownCurrent;
    int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void LoadFromScriptableObject<T>(T scriptableObject){
        // Load stuff
        throw new System.NotImplementedException();
    }
}
