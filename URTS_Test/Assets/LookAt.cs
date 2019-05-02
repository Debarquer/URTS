using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform thing;
    public Transform firePointTransform;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(thing);
        GetComponentInChildren<LineRenderer>().SetPosition(0, firePointTransform.position);
        GetComponentInChildren<LineRenderer>().SetPosition(1, thing.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
