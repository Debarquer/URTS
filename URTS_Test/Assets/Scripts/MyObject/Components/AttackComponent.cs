using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class AttackComponent : MonoBehaviour {

    public int damage = 1;
    public float attackRateMax = 2f;
    private float attackRateCurr = 0;

    MyObject myObject;

    private void Start() {
        myObject = GetComponentInParent<MyObject>();
    }

    private void Update() {
        //Debug.Log("Attacking");

        attackRateCurr += Time.deltaTime;
        if(attackRateCurr > attackRateMax) {
            var enemies = FindObjectsOfType<MonoBehaviour>().OfType<IAttackable>();
            foreach (IAttackable enemy in enemies) {
                attackRateCurr = 0;

                if ((MyObject)enemy != myObject) {
                    if (((MyObject)enemy).team != myObject.team) {
                        if (Vector3.Distance(((MyObject)enemy).transform.position, transform.position) < 10) {
                            enemy.ReceiveDamage(damage);
                            return;
                        }
                    }
                }
            }
        }
        
    }
}