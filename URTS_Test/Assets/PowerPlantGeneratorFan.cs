using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlantGeneratorFan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.RotateAroundLocal(transform.up, 1f);
        transform.Rotate(new Vector3(0, 1f, 0), Space.Self);
    }
}
