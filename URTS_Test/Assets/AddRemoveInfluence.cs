using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRemoveInfluence : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.activeSelf == false)
            return;

        MyObject myObject = other.GetComponent<MyObject>();
        if(myObject != null) {
            myObject.insideInfluence = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.activeSelf == false)
            return;

        MyObject myObject = other.GetComponent<MyObject>();
        if (myObject != null) {
            myObject.insideInfluence = false;
        }
    }
}
