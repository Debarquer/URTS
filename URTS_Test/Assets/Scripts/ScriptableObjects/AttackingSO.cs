using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attacking Scriptable Object", menuName = "Scriptable Objects/Attacking Scriptable Object", order = 1)]
public class AttackingSO : MySO
{
    public int range;
    public float attackCooldownMax;
    public float attackCooldownCurrent;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
