using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryDoor : MonoBehaviour
{
    public Transform door;
    public Transform elevator;

    Coroutine coroutine = null;

    [SerializeField] private bool open = false;

    private Vector3 openPos = new Vector3(-3, -3, -10);
    private Vector3 closedPos = new Vector3(-3, 6, -10);

    private Vector3 bottomPos = new Vector3(-3, 0, -2);
    private Vector3 topPos = new Vector3(-3, 6, -2);

    [SerializeField] GameObject unit = null;

    public GameObject lightsContainer;

    public void TurnOnLights() {
        lightsContainer.SetActive(true);
    }

    public void TurnOffLights() {
        lightsContainer.SetActive(false);
    }

    public void OpenDoor() {
        Debug.Log("OpenDoor");

        if (open) {
            return;
        }

        if(coroutine == null) {
            open = true;
            coroutine = StartCoroutine(OpenDoorCoroutine(openPos));
        }
    }

    public void CloseDoor() {
        if (!open) {
            return;
        }

        open = false;
        coroutine = StartCoroutine(CloseDoorCoroutine(closedPos));
        Invoke("TurnOffLights", 0);
    }

    IEnumerator OpenDoorCoroutine(Vector3 destination) {
        Debug.Log("OpenDoorCoroutine: " + destination);

        Vector3 startPos = door.transform.localPosition;
        for (float i = 0; i <= 1; i += Time.deltaTime) {
            door.transform.localPosition = Vector3.Lerp(startPos, destination, i);

            yield return null;
        }

        Invoke("CloseDoor", 2f);
    }

    IEnumerator CloseDoorCoroutine(Vector3 destination) {
        Debug.Log("OpenDoorCoroutine: " + destination);

        Vector3 startPos = door.transform.localPosition;
        for (float i = 0; i <= 1; i += Time.deltaTime) {
            door.transform.localPosition = Vector3.Lerp(startPos, destination, i);

            yield return null;
        }

        coroutine = null;
    }

    private void LiftLowerElevator() {
        StartCoroutine(LiftLowerElevatorCoroutine(topPos));
        //Invoke("StartCoroutine(LiftLowerElevatorCoroutine(topPos)", 2f);
    }

    IEnumerator LiftLowerElevatorCoroutine(Vector3 destination) {
        Debug.Log("LiftLowerElevatorCoroutine: " + destination);

        Vector3 startPos = elevator.transform.localPosition;
        Vector3 unitStartPos = unit.transform.localPosition;
        for (float i = 0; i <= 1; i += Time.deltaTime / 2) {
            elevator.transform.localPosition = Vector3.Lerp(bottomPos, destination, i);
            unit.transform.localPosition = Vector3.Lerp(new Vector3(unitStartPos.x, bottomPos.y, unitStartPos.z), new Vector3(unitStartPos.x, topPos.y, unitStartPos.z), i);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);
        unit = null;

        CloseDoor();
    }

    private void OnTriggerEnter(Collider other) {
        if (unit != null)
            return;

        MyObject myObject = other.GetComponent<MyObject>();
        if(myObject == null) {
            return;
        }


        Debug.Log("Adding " + other.name);
        unit = other.gameObject;
    }

    //private void OnTriggerExit(Collider other) {
    //    MyObject myObject = other.GetComponent<MyObject>();
    //    if (myObject == null) {
    //        myObject = other.GetComponentInParent<MyObject>();
    //        if (myObject == null) {
    //            return;
    //        }
    //    }

    //    unit = null;
    //}
}
