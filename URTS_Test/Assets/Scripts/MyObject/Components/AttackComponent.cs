using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class AttackComponent : MonoBehaviour {

    MyObject myObject;

    private void Start() {
        myObject = GetComponentInParent<MyObject>();
    }

    private void Update() {
        //Debug.Log("Attacking");

        var enemies = FindObjectsOfType<MonoBehaviour>().OfType<IAttackable>();
        foreach (IAttackable enemy in enemies) {
            if((MyObject)enemy != myObject) {
                if(((MyObject)enemy).team != myObject.team) {
                    if (Vector3.Distance(((MyObject)enemy).transform.position, transform.position) < 10) {
                        enemy.ReceiveDamage(1000);

                    }
                }
            }
        }
    }
}