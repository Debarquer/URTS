using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

enum AttackEffectiviness { Effective, Standard, Ineffective };
class AttackComponent : MonoBehaviour {

    public float damage = 1;
    public float attackRateMax = 2f;
    public float attackRateCurr = 0;
    public float attackRange = 15;

    public MyObject myObject;

    //LineRenderer lr;
    [SerializeField] MyObject targetEnemy;

    public MyObjectType attackType;

    //public List<AttackableComponent> targetsInRange = new List<AttackableComponent>();
    public List<AttackableComponent> effectiveTargets = new List<AttackableComponent>();
    public List<AttackableComponent> standardTargets = new List<AttackableComponent>();
    public List<AttackableComponent> ineffectiveTargets = new List<AttackableComponent>();

    public Transform firePoint;
    public GameObject weapon;

    [SerializeField] AttackableComponent targetAttackableComponent;

    public AudioSource audioSource;

    public GameObject bullet;

    private void Start() {
        myObject = GetComponentInParent<MyObject>();
    }

    private void Update() {
        if (!GetComponent<MyObject>().active)
            return;

        if (targetEnemy == null || targetAttackableComponent == null) {
            FindTargetEnemy();
        }

        attackRateCurr += Time.deltaTime;
        if (attackRateCurr > attackRateMax) {
            //if (targetEnemy == null || targetAttackableComponent == null) {
            //    FindTargetEnemy();
            //}

            if (targetEnemy != null && targetAttackableComponent != null) {
                attackRateCurr = 0;
                MoveToAndDestroy moveToAndDestroy = Instantiate(bullet, firePoint.transform.position, firePoint.rotation).GetComponent<MoveToAndDestroy>();
                moveToAndDestroy.init(targetEnemy.GetComponent<AttackableComponent>(), GetComponent<MyObject>().myObjectType, 100f, damage);
            }
            else {
                if (audioSource != null) {
                    if (audioSource.isPlaying) {
                        audioSource.Stop();
                    }
                }
            }
        }

        if (weapon != null && targetEnemy != null) {
            weapon.transform.LookAt(targetEnemy.transform);
        }
    }

    private void FindTargetEnemy() {
        List<AttackableComponent> deadenemies = new List<AttackableComponent>();

        targetEnemy = null;
        targetAttackableComponent = null;

        if (effectiveTargets != null && effectiveTargets.Count > 0) {
            if (targetEnemy == null || targetAttackableComponent == null) {
                deadenemies = FindTarget(effectiveTargets);
            }
            foreach (AttackableComponent deadenemy in deadenemies) {
                effectiveTargets.Remove(deadenemy);
            }
            deadenemies.Clear();
        }

        if (standardTargets != null && standardTargets.Count > 0) {
            if (targetEnemy == null || targetAttackableComponent == null) {
                deadenemies = FindTarget(standardTargets);
            }
            foreach (AttackableComponent deadenemy in deadenemies) {
                standardTargets.Remove(deadenemy);
            }
            deadenemies.Clear();
        }

        if (ineffectiveTargets != null && ineffectiveTargets.Count > 0) {
            if (targetEnemy == null || targetAttackableComponent == null) {
                deadenemies = FindTarget(ineffectiveTargets);
            }
            foreach (AttackableComponent deadenemy in deadenemies) {
                ineffectiveTargets.Remove(deadenemy);
            }
            deadenemies.Clear();
        }
    }

    private List<AttackableComponent> FindTarget(List<AttackableComponent> targets){
        List<AttackableComponent> deadenemies = new List<AttackableComponent>();
        foreach (AttackableComponent enemy in targets) {
            if (enemy == null) {
                deadenemies.Add(enemy);
            }
            else {
                if (Mathf.Abs((transform.position - enemy.transform.position).magnitude) > attackRange)
                    continue;

                targetAttackableComponent = enemy;
                targetEnemy = enemy.GetComponent<MyObject>();

                if (weapon != null) {
                    weapon.transform.LookAt(enemy.transform);
                }

                return deadenemies;
            }
        }

        return deadenemies;
    }

    static public float GetDamageModifier(MyObjectType attackType, MyObjectType defenceType) {
        AttackEffectiviness attackEffectiviness = GetEffectiviness(attackType, defenceType);

        switch (attackEffectiviness) {
            case AttackEffectiviness.Effective:
                return 2f;
            case AttackEffectiviness.Standard:
                return 1f;
            case AttackEffectiviness.Ineffective:
                return 0.5f;
            default:
                return 1f;
        }
    }

    public void AddTargetInRange(AttackableComponent attackableComponent, MyObjectType targetObjectType) {
        if (effectiveTargets.Contains(attackableComponent) || standardTargets.Contains(attackableComponent) || ineffectiveTargets.Contains(attackableComponent))
            return;

        AttackEffectiviness attackEffectiviness = GetEffectiviness(attackType, targetObjectType);

        switch (attackEffectiviness) {
            case AttackEffectiviness.Effective:
                effectiveTargets.Add(attackableComponent);
                break;
            case AttackEffectiviness.Standard:
                standardTargets.Add(attackableComponent);
                break;
            case AttackEffectiviness.Ineffective:
                ineffectiveTargets.Add(attackableComponent);
                break;
        }
    }

    public void RemoveTargetInRange(AttackableComponent attackableComponent, MyObjectType targetObjectType) {

        AttackEffectiviness attackEffectiviness = GetEffectiviness(attackType, targetObjectType);

        switch (attackEffectiviness) {
            case AttackEffectiviness.Effective:
                effectiveTargets.Remove(attackableComponent);
                break;
            case AttackEffectiviness.Standard:
                standardTargets.Remove(attackableComponent);
                break;
            case AttackEffectiviness.Ineffective:
                ineffectiveTargets.Remove(attackableComponent);
                break;
        }
    }

    static public AttackEffectiviness GetEffectiviness(MyObjectType attackType, MyObjectType defenceType) {
        if (attackType == MyObjectType.Light && defenceType == MyObjectType.Heavy) {
            return AttackEffectiviness.Ineffective;
        }
        else if (attackType == MyObjectType.Heavy && defenceType == MyObjectType.Light) {
            return AttackEffectiviness.Ineffective;
        }
        else if (attackType == MyObjectType.Light && defenceType == MyObjectType.Light) {
            return AttackEffectiviness.Effective;
        }
        else if (attackType == MyObjectType.Heavy && defenceType == MyObjectType.Heavy) {
            return AttackEffectiviness.Effective;
        }
        else {
            return AttackEffectiviness.Standard;
        }
    }
}