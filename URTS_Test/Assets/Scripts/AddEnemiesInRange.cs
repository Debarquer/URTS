using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEnemiesInRange : MonoBehaviour
{

    AttackComponent attackComponent;

    private void OnTriggerEnter(Collider other) {

        if(attackComponent == null) {
            attackComponent = GetComponentInParent<AttackComponent>();
        }

        AttackableComponent attackableComponent = other.GetComponent<AttackableComponent>();
        if (attackableComponent != null) {
            MyObject enemyObject = attackableComponent.GetComponent<MyObject>();
            if (!enemyObject.placed) {
                // Building has not been placed
                return;
            }

            if (enemyObject != attackComponent.myObject) {
                if (enemyObject.team != attackComponent.myObject.team) {
                    attackComponent.AddTargetInRange(attackableComponent, enemyObject.myObjectType);
                }
            }
        }
        else {
            attackableComponent = other.GetComponentInParent<AttackableComponent>();
            if (attackableComponent != null) {
                MyObject enemyObject = attackableComponent.GetComponentInParent<MyObject>();
                if (!enemyObject.placed) {
                    // Building has not been placed
                    return;
                }

                if (enemyObject != attackComponent.myObject) {
                    if (enemyObject.team != attackComponent.myObject.team) {
                        attackComponent.AddTargetInRange(attackableComponent, enemyObject.myObjectType);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (attackComponent == null) {
            attackComponent = GetComponentInParent<AttackComponent>();
        }

        AttackableComponent attackableComponent = other.GetComponent<AttackableComponent>();
        if (attackableComponent != null) {
            MyObject enemyObject = attackableComponent.GetComponent<MyObject>();

            if (enemyObject != attackComponent.myObject) {
                if (enemyObject.team != attackComponent.myObject.team) {
                    attackComponent.RemoveTargetInRange(attackableComponent, enemyObject.myObjectType);
                }
            }
        }
        else {
            attackableComponent = other.GetComponentInParent<AttackableComponent>();
            if (attackableComponent != null) {
                MyObject enemyObject = attackableComponent.GetComponentInParent<MyObject>();

                if (enemyObject != attackComponent.myObject) {
                    if (enemyObject.team != attackComponent.myObject.team) {
                        attackComponent.RemoveTargetInRange(attackableComponent, enemyObject.myObjectType);
                    }
                }
            }
        }
    }
}
