using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum team { A, B, C, D };
public class GameManager : MonoBehaviour
{

    List<MoveComponent> moveComponents;

    Vector3 dragStartPos;
    Vector3 dragEndPos;
    float dragTimer = 0f;

    List<MyObject> selectedObjects = new List<MyObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            dragStartPos = hit.point;

            //foreach(MoveComponent mc in moveComponents) {
            //    mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
            //}
        }
        if (Input.GetMouseButton(0)) {
            dragTimer += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0)) {
            if(dragTimer < 0.1f) {
                foreach (MyObject selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                    }
                }
                selectedObjects.Clear();
                dragTimer = 0;
                return;
            }
            dragTimer = 0;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            dragEndPos = hit.point;

            if(dragStartPos != null && dragEndPos != null) {
                Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
                Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z));
                Collider[] objectHoveredOver = Physics.OverlapBox(midPoint, extents);

                Debug.Log("Mid: " + midPoint + ": extends: " + (dragStartPos - midPoint));
                

                foreach (Collider c in objectHoveredOver) {
                    MyObject myObject = c.GetComponent<MyObject>();
                    if(myObject != null) {
                        selectedObjects.Add(myObject);
                        c.GetComponentInChildren<Renderer>().material.color = Color.black;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            dragStartPos = hit.point;

            moveComponents = new List<MoveComponent>(FindObjectsOfType<MoveComponent>());

            foreach (MyObject selectedObject in selectedObjects) {
                MoveComponent mc = selectedObject.GetComponent<MoveComponent>();
                if(mc != null) {
                    mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
                }
            }
        }
    }
}
