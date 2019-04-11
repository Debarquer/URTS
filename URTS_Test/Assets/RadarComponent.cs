using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<MyObject>().OnActivate += EnableRadar;
        transform.GetComponent<MyObject>().OnMyObjectDisable += DisableRadar;
    }

    private void OnDisable() {
        DisableRadar();
    }

    private void EnableRadar() {
        FindObjectOfType<GameManager>().EnableRadar(true);
    }

    private void DisableRadar() {
        FindObjectOfType<GameManager>().EnableRadar(false);
    }
}
