using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public float maxMinerals = 1000;
    float currentMinerals;

    public List<GameObject> gathererPoints;
    private Dictionary<GameObject, bool> isPointAvailable = new Dictionary<GameObject, bool>();

    // Start is called before the first frame update
    void Start()
    {
        currentMinerals = maxMinerals;

        foreach(GameObject gathererPoint in gathererPoints) {
            isPointAvailable[gathererPoint] = true;
        }
    }

    public void Mine(float amount) {
        currentMinerals -= amount;
        if(currentMinerals <= 0) {
            Destroy(gameObject);
        }
    }

    public GameObject GetGathererPoint() {
        foreach(GameObject gathererPoint in gathererPoints) {
            if (isPointAvailable[gathererPoint]) {
                isPointAvailable[gathererPoint] = false;
                return gathererPoint;
            }
        }

        return null;
    }

    public void FreeUpGathererPoint(GameObject gathererPoint) {
        isPointAvailable[gathererPoint] = true;
    }
}
