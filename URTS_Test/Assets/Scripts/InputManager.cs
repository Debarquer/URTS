using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InputMode { StandardInGame, BuildingPlacement, SellBuildings };
public class InputManager : MonoBehaviour
{
    #region General variables
    public LayerMask groundLayerMask;
    protected Camera mainCamera;

    protected Vector3 dragStartPos;
    protected Vector3 dragEndPos;
    protected float dragTimer = 0f;
    protected Vector3 _lastMouseGroundPlanePosition;
    public Vector3 _lastMousePosition;

    public float cameraTranslateXMax = 1850, cameraTranslateXMin = 100, cameraTranslateYMax = 1000, cameraTranslateYMin = 100;
    public float cameraPosMaxX = 199, cameraPosMinX = -29, cameraPosMaxZ = 298, cameraPosMinZ = -49;

    public Canvas HoverIdentifier;
    float hoverTimrMax = 0.75f;
    float hoverTimerCurr = 0;
    #endregion

    #region InputMode.StandardInGame Variables
    [SerializeField] protected List<SelectableComponent> selectedObjects = new List<SelectableComponent>();

    protected InputMode inputMode = InputMode.StandardInGame;

    public Image selectionImage;
    #endregion

    #region InputMode.BuildingPlacement Variables

    protected MyObject buildingToBePlaced = null;
    protected GameObject gameObjectToBePlaced = null;
    protected float cameraTranslateSpeed = 30f;

    public object HandleSellBuildings { get; private set; }

    public delegate void BuildingPlacedSuccessfullyDelegate();
    public event BuildingPlacedSuccessfullyDelegate OnBuildingPlacedSuccessfully;

    #endregion

    // Start is called before the first frame update
    virtual protected void Start()
    {
        mainCamera = Camera.main;

        _lastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu") {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        else if (Input.GetKeyUp(KeyCode.O)) {
            ToggleSellBuildings();
        }

        HandleCameraMovement();

        MyObject myObject = RaycastForMyObject();
        if(myObject != null) {
            hoverTimerCurr += Time.deltaTime;
            if(hoverTimerCurr >= hoverTimrMax) {
                if (!HoverIdentifier.gameObject.activeSelf) {
                    HoverIdentifier.gameObject.SetActive(true);
                }
                HoverIdentifier.GetComponentInChildren<Text>().text = myObject.myName;
                HoverIdentifier.GetComponentInChildren<Image>().transform.position = Input.mousePosition + new Vector3(100, 30, 0);
            }         
        }
        else {
            hoverTimerCurr = 0;

            if (HoverIdentifier.gameObject.activeSelf) {
                HoverIdentifier.gameObject.SetActive(false);
            }
        }

        switch (inputMode) {
            case InputMode.StandardInGame:
                HandleStandardInput();
                break;
            case InputMode.BuildingPlacement:
                HandleBuildingPlacementInput();
                break;
            case InputMode.SellBuildings:
                HandleSellBuildingsInput();
                break;
        }
    }

    protected void HandleCameraMovement() {
        Vector3 mouseTranslateMove = Vector3.zero;

        if (Input.GetMouseButton(2)) {
            Update_CameraDrag();
        }
        //    float cameraTranslateXMax = 60, cameraTranslateXMin = -20, cameraTranslateYMax = 25, cameraTranslateYMin = -15;
        else if (Input.mousePosition.x >= cameraTranslateXMax || Input.mousePosition.x <= cameraTranslateXMin || Input.mousePosition.y >= cameraTranslateYMax || Input.mousePosition.y <= cameraTranslateYMin) {
            if (Input.mousePosition.x >= cameraTranslateXMax) {
                mouseTranslateMove.x = Input.mousePosition.x;
                mainCamera.transform.Translate(new Vector3(1, 0, 0) * cameraTranslateSpeed * Time.deltaTime);
                mouseTranslateMove.x = 0;
            }
            if (Input.mousePosition.x <= cameraTranslateXMin) {
                mouseTranslateMove.x = Input.mousePosition.x;
                mainCamera.transform.Translate(new Vector3(-1, 0, 0) * cameraTranslateSpeed * Time.deltaTime);
                mouseTranslateMove.x = 0;
            }
            if (Input.mousePosition.y >= cameraTranslateYMax) {
                mouseTranslateMove.y = Input.mousePosition.y;
                mainCamera.transform.Translate(new Vector3(0, 1, 0) * cameraTranslateSpeed * Time.deltaTime);
                mouseTranslateMove.y = 0;
            }
            if (Input.mousePosition.y <= cameraTranslateYMin) {
                mouseTranslateMove.y = Input.mousePosition.y;
                mainCamera.transform.Translate(new Vector3(0, -1, 0) * cameraTranslateSpeed * Time.deltaTime);
                mouseTranslateMove.y = 0;
            }
        }
        else {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            mainCamera.transform.Translate(new Vector3(x, z, 0) * cameraTranslateSpeed * Time.deltaTime);
        }
        Update_ScrollZoom();

        float cameraClampedX = Mathf.Clamp(mainCamera.transform.position.x, cameraPosMinX, cameraPosMaxX);
        float cameraClampedZ = Mathf.Clamp(mainCamera.transform.position.z, cameraPosMinZ, cameraPosMaxZ);
        mainCamera.transform.position = new Vector3(cameraClampedX, mainCamera.transform.position.y, cameraClampedZ);

        _lastMouseGroundPlanePosition = RaycastToGround().point;
        _lastMousePosition = Input.mousePosition;
    }

    public void Deselect(MyObject objectToDeselect) {
        selectedObjects.Remove(objectToDeselect.GetComponent<SelectableComponent>());
    }

    protected void InitializeStandardInput() {
        inputMode = InputMode.StandardInGame;
    }

    protected void HandleStandardInput() {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

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
            //if (dragTimer < 0.1f) {
            //    selectionImage.enabled = false;

            //    foreach (SelectableComponent selectedObject in selectedObjects) {
            //        if (selectedObject != null) {
            //            //selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
            //            selectedObject.Deselect();
            //        }
            //    }
            //    selectedObjects.Clear();
            //    dragTimer = 0;

            //    //ClickableComponent[] clickableComponents = RaycastForClickableComponents();
            //    //if (clickableComponents != null && clickableComponents.Length > 0)
            //    //    ClickableComponentManager.instance.UnclickAllClickableComponents();
            //    //foreach (ClickableComponent clickableComponent in clickableComponents) {
            //    //    clickableComponent.Click();
            //    //}

            //    return;
            //}
            dragTimer = 0;

            EndDrag();
        }
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit = RaycastToGround();

            dragStartPos = hit.point;

            foreach (SelectableComponent selectedObject in selectedObjects) {
                MoveComponent mc = selectedObject.GetComponent<MoveComponent>();
                if (mc != null) {
                    //mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
                    mc.SetAgentDestination(hit.point);
                }
            }
        }
    }

    protected void EndDrag() {
        selectionImage.enabled = false;

        RaycastHit hit = RaycastToGround();

        dragEndPos = hit.point;

        if (dragStartPos != null && dragEndPos != null) {
            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), 100, Mathf.Abs((dragStartPos - midPoint).z));
            Collider[] objectsHoveredOver = Physics.OverlapBox(midPoint, extents);

            foreach (Collider c in objectsHoveredOver) {
                SelectableComponent selectableComponent = c.GetComponent<SelectableComponent>();
                if (selectableComponent != null) {
                    selectedObjects.Add(selectableComponent);
                    selectableComponent.GetComponent<MyObject>().OnGameObjectDisabled += Deselect;

                    selectableComponent.Select();
                }
            }
        }
    }

    protected void StartDrag() {
        DeselectAll();

        selectionImage.enabled = true;

        RaycastHit hit = RaycastToGround();

        dragStartPos = hit.point;
    }

    private void DeselectAll() {
        foreach (SelectableComponent selectableComponent in selectedObjects) {
            selectableComponent.Deselect();
        }
        selectedObjects.Clear();
    }

    protected ClickableComponent[] RaycastForClickableComponents() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.black, 1f);

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

    protected MyObject RaycastForMyObject() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.black, 1f);

        foreach (RaycastHit hit in hits) {
            //if (hit.transform.name == "AddEnemiesInRange")
            //    continue;

            MyObject myObject = hit.transform.GetComponent<MyObject>();
            if (myObject != null) {
                return myObject;
            }
            //else {
            //    myObject = hit.transform.GetComponentInParent<MyObject>();
            //    if (myObject != null) {
            //        return myObject;
            //    }
            //}
        }

        return null;
    }

    protected RaycastHit RaycastToGround() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask);
        return hit;
    }

    public void InitiateBuildingPlacement(MyObject buildingToBePlaced) {
        this.buildingToBePlaced = buildingToBePlaced;
        inputMode = InputMode.BuildingPlacement;
        FindObjectOfType<AddRemoveInfluenceManager>().ShowAddRemoveInfluences();
    }

    public void EndBuildingPlacement(bool success) {
        FindObjectOfType<AddRemoveInfluenceManager>().HideAddRemoveInfluences();

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

    protected void HandleBuildingPlacementInput() {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        RaycastHit hit = RaycastToGround();
        if(gameObjectToBePlaced == null) {
            gameObjectToBePlaced = Instantiate(buildingToBePlaced.gameObject, hit.point, Quaternion.identity);
        }
        else {
            gameObjectToBePlaced.transform.position = hit.point;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (gameObjectToBePlaced.GetComponent<MyObject>().insideInfluence && hit.transform.name != "NoBuildZone") {
                EndBuildingPlacement(true);
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            EndBuildingPlacement(false);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            gameObjectToBePlaced.transform.Rotate(Vector3.up, -90);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            gameObjectToBePlaced.transform.Rotate(Vector3.up, 90);
        }
    }

    protected void HandleSellBuildingsInput() {
        if (Input.GetMouseButtonUp(0)) {
            ClickableComponent[] clickableComponents = RaycastForClickableComponents();
            foreach (ClickableComponent clickableComponent in clickableComponents) {
                MyObject myObject = clickableComponent.GetComponent<MyObject>();
                if (myObject != null && myObject.team == Team.A) {
                    FindObjectOfType<GameManager>().UpdateMinerals(myObject.cost * 0.9f);
                    Destroy(myObject.gameObject);
                }

            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            InitializeStandardInput();
        }
    }

    protected void Update_CameraDrag() {
        Vector3 hitPos = RaycastToGround().point;

        Vector3 diff = _lastMouseGroundPlanePosition - hitPos;
        mainCamera.transform.Translate(diff, Space.World);

        _lastMouseGroundPlanePosition = hitPos = RaycastToGround().point;
    }

    protected void Update_ScrollZoom() {
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > 0.01f) {

            float minHeight = 10;
            float maxHeight = 75;

            Vector3 hitPos = RaycastToGround().point;

            // Move camera towards hitPos
            Vector3 dir = hitPos - mainCamera.transform.position;

            Vector3 p = mainCamera.transform.position;

            // Stop zooming out at a certain distance.
            if (dir.y * scrollAmount < 0 || p.y < maxHeight) {
                mainCamera.transform.Translate(dir * scrollAmount, Space.World);
            }

            p = mainCamera.transform.position;
            if (p.y < minHeight) {
                p.y = minHeight;
            }
            if (p.y > maxHeight) {
                p.y = maxHeight;
            }
            mainCamera.transform.position = p;
        }
    }

    public void ToggleSellBuildings() {
        if(inputMode == InputMode.SellBuildings) {
            InitializeStandardInput();
        }
        else {
            inputMode = InputMode.SellBuildings;
        }
    }
}
