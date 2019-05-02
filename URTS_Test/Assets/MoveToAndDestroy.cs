using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToAndDestroy : MonoBehaviour
{
    AttackableComponent target;
    float speed = 21f;
    float damage = 1f;
    MyObjectType damageType;

    public GameObject hitSound;

    LineRenderer lineRenderer;

    Coroutine coroutine;

    public void init(AttackableComponent target, MyObjectType damageType, float speed = 21f, float damage = 1) {
        this.target = target;
        this.damageType = damageType;
        this.speed = speed;
        this.damage = damage;

        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null) {
            //transform.LookAt(target.transform.position);
            //transform.localRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            //transform.Translate(transform.forward * speed * Time.deltaTime);

            if(coroutine == null)
                coroutine = StartCoroutine(MoveTowardsTarget());
        }
        else {
            if(coroutine != null)
                StopCoroutine(coroutine);
            Destroy(gameObject);
        }
    }

    IEnumerator MoveTowardsTarget() {
        Vector3 startPos = transform.position;

        for(float i = 0; i <= 1f; i+= Time.deltaTime * 2f) {
            transform.position = Vector3.Lerp(startPos, target.transform.position, i);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward);

            if (Vector3.Distance(transform.position, target.transform.position) < 1f) {
                target.ReceiveDamage(damage * AttackComponent.GetDamageModifier(damageType, target.GetComponent<MyObject>().myObjectType));
                if (hitSound != null) {
                    Instantiate(hitSound, transform.position, Quaternion.identity);
                }
                target = null;
                //Destroy(gameObject);
            }

            yield return null;
        }
    }
}
