using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "Scriptable Objects/Scriptable Object", order = 1)]
public class MySO : ScriptableObject
{
    public float moveSpeed;
    public int cost;
    // graphics
    public int currHealth;
    public int maxHealth;
    public string myName;
    public string description;
    public MyObjectType myObjectType;
    System.Type gameObjectType; //game object type eg. MyObject, AttackingObject, SpawningUnit etc
}
