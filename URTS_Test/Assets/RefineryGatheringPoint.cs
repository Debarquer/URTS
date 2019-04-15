using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineryGatheringPoint : MonoBehaviour
{

    public MyObject gathererPrefab;

    bool available = true;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<MyObject>().OnActivate += InstantiateFreeGatherer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiateFreeGatherer() {
        Instantiate(gathererPrefab, transform.position, Quaternion.identity);
        GetComponentInParent<MyObject>().OnActivate -= InstantiateFreeGatherer;
    }

    public bool IsAvailable() {
        return available;
    }

    public void Occupy() {
        available = false;
    }

    public void FreeUp() {
        available = true;
    }
}
