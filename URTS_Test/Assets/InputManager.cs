﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    List<SelectableComponent> selectedObjects = new List<SelectableComponent>();

    InputMode inputMode = InputMode.StandardInGame;

    public Image selectionImage;

    #endregion

    #region InputMode.BuildingPlacement Variables

    MyObject buildingToBePlaced = null;
    GameObject gameObjectToBePlaced = null;
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
        selectedObjects.Remove(objectToDeselect.GetComponent<SelectableComponent>());
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

            RaycastHit hit = RaycastToGround();

            dragEndPos = hit.point;

            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z)) * 2;

            selectionImage.transform.position = midPoint;
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, extents.x);
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, extents.z);
        }
        if (Input.GetMouseButtonUp(0)) {
            if (dragTimer < 0.1f) {
                selectionImage.enabled = false;

                foreach (SelectableComponent selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        //selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                        selectedObject.Deselect();
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

            foreach (SelectableComponent selectedObject in selectedObjects) {
                MoveComponent mc = selectedObject.GetComponent<MoveComponent>();
                if (mc != null) {
                    mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
                }
            }
        }
        if (Input.GetMouseButton(2)) {
            Update_CameraDrag();
        }
        Update_ScrollZoom();

        _lastMouseGroundPlanePosition = RaycastToGround().point;
    }

    private void EndDrag() {
        selectionImage.enabled = false;

        RaycastHit hit = RaycastToGround();

        dragEndPos = hit.point;

        if (dragStartPos != null && dragEndPos != null) {
            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z));
            Collider[] objectHoveredOver = Physics.OverlapBox(midPoint, extents);

            foreach (Collider c in objectHoveredOver) {
                SelectableComponent myObject = c.GetComponent<SelectableComponent>();
                if (myObject != null) {
                    selectedObjects.Add(myObject);
                    myObject.GetComponent<MyObject>().OnGameObjectDisabled += Deselect;

                    myObject.Select();
                }
            }
        }
    }

    private void StartDrag() {
        selectionImage.enabled = true;

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
            gameObjectToBePlaced.GetComponent<MyObject>().OnPlaced();
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
            gameObjectToBePlaced = Instantiate(buildingToBePlaced.gameObject, hit.point, Quaternion.identity);
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

    void Update_ScrollZoom() {
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > 0.01f) {

            float minHeight = 10;
            float maxHeight = 75;

            Vector3 hitPos = RaycastToGround().point;

            // Move camera towards hitPos
            Vector3 dir = hitPos - Camera.main.transform.position;

            Vector3 p = Camera.main.transform.position;

            // Stop zooming out at a certain distance.
            // TODO: Maybe you should still slide around at 20 zoom?
            if (dir.y * scrollAmount < 0 || p.y < maxHeight) {
                Camera.main.transform.Translate(dir * scrollAmount, Space.World);
            }

            p = Camera.main.transform.position;
            if (p.y < minHeight) {
                p.y = minHeight;
            }
            if (p.y > maxHeight) {
                p.y = maxHeight;
            }
            Camera.main.transform.position = p;
        }
    }
}
