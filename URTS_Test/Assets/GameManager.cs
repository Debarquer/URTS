using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    List<AttackingUnit> attackingUnits;

    // Start is called before the first frame update
    void Start()
    {
        attackingUnits = new List<AttackingUnit>(FindObjectsOfType<AttackingUnit>());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            foreach(AttackingUnit au in attackingUnits) {
                au.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
            }
        }
    }
}
