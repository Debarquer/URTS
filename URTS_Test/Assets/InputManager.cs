using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InputMode { StandardInGame, BuildingPlacement };
public class InputManager : MonoBehaviour
{
    #region General variables
    public LayerMask groundLayerMask;
    #endregion

    #region InputMode.StandardInGame Variables
    Vector3 dragStartPos;
    Vector3 dragEndPos;
    float dragTimer = 0f;

    List<MyObject> selectedObjects = new List<MyObject>();

    InputMode inputMode = InputMode.StandardInGame;

    #endregion

    #region InputMode.BuildingPlacement Variables

    MyObject buildingToBePlaced = null;
    MyObject gameObjectToBePlaced = null;
    private Vector3 _lastMouseGroundPlanePosition;

    public delegate void BuildingPlacedSuccessfullyDelegate();
    public event BuildingPlacedSuccessfullyDelegate OnBuildingPlacedSuccessfully;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (inputMode) {
            case InputMode.StandardInGame:
                HandleStandardInput();
                break;
            case InputMode.BuildingPlacement:
                HandleBuildingPlacementInput();
                break;
        }
    }

    public void Deselect(MyObject objectToDeselect) {
        selectedObjects.Remove(objectToDeselect);
    }

    private void InitializeStandardInput() {
        inputMode = InputMode.StandardInGame;
    }

    private void HandleStandardInput() {

        if (Input.GetMouseButtonDown(0)) {
            StartDrag();
        }
        if (Input.GetMouseButton(0)) {
            dragTimer += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0)) {
            if (dragTimer < 0.1f) {
                foreach (MyObject selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                    }
                }
                selectedObjects.Clear();
                dragTimer = 0;

                ClickableComponent[] clickableComponents = RaycastForClickableComponents();
                if (clickableComponents != null && clickableComponents.Length > 0)
                    ClickableComponentManager.instance.UnclickAllClickableComponents();
                foreach (ClickableComponent clickableComponent in clickableComponents) {
                    clickableComponent.Click();
                }

                return;
            }
            dragTimer = 0;

            EndDrag();
        }
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit = RaycastToGround();

            dragStartPos = hit.point;

            foreach (MyObject selectedObject in selectedObjects) {
                MoveComponent mc = selectedObject.GetComponent<MoveComponent>();
                if (mc != null) {
                    mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
                }
            }
        }
        if (Input.GetMouseButton(2)) {
            Update_CameraDrag();
        }

        _lastMouseGroundPlanePosition = RaycastToGround().point;
    }

    private void EndDrag() {
        RaycastHit hit = RaycastToGround();

        dragEndPos = hit.point;

        if (dragStartPos != null && dragEndPos != null) {
            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z));
            Collider[] objectHoveredOver = Physics.OverlapBox(midPoint, extents);

            foreach (Collider c in objectHoveredOver) {
                MyObject myObject = c.GetComponent<MyObject>();
                if (myObject != null) {
                    selectedObjects.Add(myObject);
                    myObject.OnGameObjectDisabled += Deselect;
                    c.GetComponentInChildren<Renderer>().material.color = Color.black;
                }
            }
        }
    }

    private void StartDrag() {
        RaycastHit hit = RaycastToGround();

        dragStartPos = hit.point;
    }

    private ClickableComponent[] RaycastForClickableComponents() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        List<ClickableComponent> clickableComponents = new List<ClickableComponent>();

        foreach (RaycastHit hit in hits) {
            ClickableComponent clickableComponent = hit.transform.GetComponent<ClickableComponent>();
            if(clickableComponent != null) {
                clickableComponents.Add(clickableComponent);
            }
            else {
                clickableComponent = hit.transform.GetComponentInParent<ClickableComponent>();
                if (clickableComponent != null) {
                    clickableComponents.Add(clickableComponent);
                }
            }
        }

        return clickableComponents.ToArray();
    }

    private RaycastHit RaycastToGround() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask);
        return hit;
    }

    public void InitiateBuildingPlacement(MyObject buildingToBePlaced) {
        this.buildingToBePlaced = buildingToBePlaced;
        inputMode = InputMode.BuildingPlacement;
    }

    public void EndBuildingPlacement(bool success) {
        if (success) {
            // Activate building
            gameObjectToBePlaced.GetComponent<MyObject>().Activate();
            // Clean up variables
            gameObjectToBePlaced = null;
            buildingToBePlaced = null;
            // Place building
            // Reset building cooldown
            OnBuildingPlacedSuccessfully?.Invoke();
            // Reset game mode
            InitializeStandardInput();
        }
        else {
            // Clean up variables
            Destroy(gameObjectToBePlaced);
            gameObjectToBePlaced = null;
            buildingToBePlaced = null;
            // Reset game mode
            InitializeStandardInput();
        }
    }

    private void HandleBuildingPlacementInput() {
        RaycastHit hit = RaycastToGround();
        if(gameObjectToBePlaced == null) {
            gameObjectToBePlaced = Instantiate(buildingToBePlaced, hit.point, Quaternion.identity);
        }
        else {
            gameObjectToBePlaced.transform.position = hit.point;
        }

        if (Input.GetMouseButtonDown(0)) {
            EndBuildingPlacement(true);
        }
        if (Input.GetMouseButtonDown(1)) {
            EndBuildingPlacement(false);
        }
    }

    void Update_CameraDrag() {
        //if (Input.GetMouseButtonUp(0)) {
        //    CancelUpdateFunc();
        //    return;
        //}

        Vector3 hitPos = RaycastToGround().point;
        //_lastMouseGroundPlanePosition = hitPos;

        Vector3 diff = _lastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        _lastMouseGroundPlanePosition = hitPos = RaycastToGround().point;
    }
}
